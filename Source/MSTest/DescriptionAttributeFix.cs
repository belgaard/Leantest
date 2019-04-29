namespace LeanTest.MSTest
{
	/// <summary>Pre- and post-fixes for information put in .trx-files.</summary>
	public static class DescriptionAttributeFix
	{
		/// <summary>A prefix put in a .trx-file before each tag value.</summary>
		public const string Prefix = @"Description = ###---";
		/// <summary>A postfix put in a .trx-file after each tag value.</summary>
		public const string Postfix = @"---###";
	}
}