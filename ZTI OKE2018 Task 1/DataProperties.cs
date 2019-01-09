using System;

namespace ZTI_OKE2018_Task_1
{
	public partial class Data
	{
		public class DataProperties
		{
			public DataProperties(string text, int startIndex, int stopIndex, bool inDBpedia = false)
			{
				Text = text ?? throw new ArgumentNullException(nameof(text));
				StartIndex = startIndex;
				StopIndex = stopIndex;
				InDBpedia = inDBpedia;
			}

			public string Text { get; }

			public int StartIndex { get; }

			public int StopIndex { get; }

			public bool InDBpedia { get; set; }

			public string DBpediaREF
			{
				get => _DBpediaREF;
				set
				{
					_DBpediaREF = value;
					InDBpedia = true;
				}
			}

			private string _DBpediaREF;

			public override string ToString()
			{
				return ($"Nazwa: {Text}" + Environment.NewLine + $"Start Index: {StartIndex}" + Environment.NewLine + $"Stop Index: {StopIndex}" + Environment.NewLine);
			}
		}
	}
}