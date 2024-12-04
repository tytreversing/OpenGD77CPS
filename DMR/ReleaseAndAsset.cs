namespace DMR;

public class ReleaseAndAsset
{
	public GithubRelease Release { get; set; }

	public GithubReleaseAssets Asset { get; set; }

	public ReleaseAndAsset(GithubRelease release, GithubReleaseAssets asset)
	{
		Release = release;
		Asset = asset;
	}
}
