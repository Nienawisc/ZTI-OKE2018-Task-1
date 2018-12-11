using System;

namespace ZTI_OKE2018_Task_1
{
	public partial class Data
	{
		public class Triplet
		{
			public Triplet(string text, int startIndex, int stopIndex)
			{
				Text = text ?? throw new ArgumentNullException(nameof(text));
				StartIndex = startIndex;
				StopIndex = stopIndex;
			}

			public string Text { get; }

			public int StartIndex { get; }

			public int StopIndex { get; }

			public override string ToString()
			{
				return ($"Nazwa: {Text}" + Environment.NewLine + $"Start Index: {StartIndex}" + Environment.NewLine + $"Stop Index: {StopIndex}" + Environment.NewLine);
			}
		}
	}
}