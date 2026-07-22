namespace NetEvolve.Defaults.Tests.Unit;

using System.Collections.Generic;

internal class SupportDetectContinuousIntegrationTests
{
    private static readonly string PropsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportDetectContinuousIntegration.props"
    );
    private static readonly string TargetsFile = MSBuildProjectFixture.InBuildMultiTargeting(
        "SupportDetectContinuousIntegration.targets"
    );

    [Test]
    public async Task NoCiVariables_IsContinuousIntegrationIsFalse_AndNoForcedProperties()
    {
        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile]);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("ContinuousIntegrationBuild")).IsEqualTo("");
        await Assert.That(evaluated.GetProperty("Deterministic")).IsEqualTo("");
        await Assert.That(evaluated.GetProperty("RestorePackagesWithLockFile")).IsEqualTo("");
    }

    [Test]
    public async Task GenericCI_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["CI"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task GitHubActions_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["GITHUB_ACTIONS"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task GitHubActions_BranchPush_RepositoryBranchResolvesToBranchName()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["GITHUB_ACTIONS"] = "true",
            ["GITHUB_REF"] = "refs/heads/main",
            ["PublishRepositoryUrl"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile, TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("RepositoryBranch")).IsEqualTo("main");
    }

    [Test]
    public async Task GitHubActions_PullRequest_RepositoryBranchResolvesToPrNumber()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["GITHUB_ACTIONS"] = "true",
            ["GITHUB_REF"] = "refs/pull/123/merge",
            ["PublishRepositoryUrl"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile, TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("RepositoryBranch")).IsEqualTo("pr123");
    }

    [Test]
    public async Task AzureDevOps_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["TF_BUILD"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task AzureDevOps_BranchPush_RepositoryBranchResolvesToBranchName()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["TF_BUILD"] = "true",
            ["BUILD_SOURCEBRANCH"] = "refs/heads/develop",
            ["PublishRepositoryUrl"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile, TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("RepositoryBranch")).IsEqualTo("develop");
    }

    [Test]
    public async Task MultipleCiVariablesSimultaneously_StillJustDetectsTrue()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["CI"] = "true",
            ["GITHUB_ACTIONS"] = "true",
            ["TF_BUILD"] = "false",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task IsContinuousIntegrationTrue_ForcesDeterminismPropertiesRegardlessOfPreSetValues()
    {
        // Deterministic/RestorePackagesWithLockFile/ContinuousIntegrationBuild are pre-set via preSetProperties
        // (plain <PropertyGroup> before the <Import>) so the props file's unconditioned assignment inside the
        // 'IsContinuousIntegration == true' block actually gets a chance to override them, the way it would for
        // a real consuming project. CI itself only needs to be read, so it stays a global property.
        var globalProperties = new Dictionary<string, string> { ["CI"] = "true" };
        var preSetProperties = new Dictionary<string, string>
        {
            ["Deterministic"] = "false",
            ["RestorePackagesWithLockFile"] = "true",
            ["ContinuousIntegrationBuild"] = "false",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties, preSetProperties);

        await Assert.That(evaluated.GetProperty("Deterministic")).IsEqualTo("true");
        await Assert.That(evaluated.GetProperty("RestorePackagesWithLockFile")).IsEqualTo("false");
        await Assert.That(evaluated.GetProperty("ContinuousIntegrationBuild")).IsEqualTo("true");
    }

    [Test]
    public async Task CircleCI_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["CIRCLECI"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task CircleCI_PullRequest_RepositoryBranchResolvesToPrNumber()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["CIRCLECI"] = "true",
            ["CIRCLE_PR_NUMBER"] = "42",
            ["PublishRepositoryUrl"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile, TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("RepositoryBranch")).IsEqualTo("pr42");
    }

    [Test]
    public async Task GitLabCI_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["GITLAB_CI"] = "true" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task GitLabCI_MergeRequest_RepositoryBranchResolvesToPrNumber()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["GITLAB_CI"] = "true",
            ["CI_MERGE_REQUEST_IID"] = "7",
            ["PublishRepositoryUrl"] = "true",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile, TargetsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("RepositoryBranch")).IsEqualTo("pr7");
    }

    [Test]
    public async Task Jenkins_JenkinsUrlVariable_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string> { ["JENKINS_URL"] = "https://jenkins.example.com" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task Jenkins_BuildIdAndBuildUrlVariables_DetectsTrue()
    {
        var globalProperties = new Dictionary<string, string>
        {
            ["BUILD_ID"] = "1",
            ["BUILD_URL"] = "https://jenkins.example.com/job/1",
        };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("true");
    }

    [Test]
    public async Task Jenkins_OnlyBuildIdWithoutBuildUrl_DoesNotDetect()
    {
        var globalProperties = new Dictionary<string, string> { ["BUILD_ID"] = "1" };

        using var evaluated = MSBuildProjectFixture.Evaluate("Foo", [PropsFile], globalProperties);

        await Assert.That(evaluated.GetProperty("IsContinuousIntegration")).IsEqualTo("false");
    }
}
