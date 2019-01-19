using System;
using System.Linq;

namespace ZTI_OKE2018_Task_1
{
	/// <summary>
	///     Main class of output data
	/// </summary>
	public class OutputData
	{
		/// <summary>
		/// Proper reference to the DBPedia
		/// </summary>
		private string _taIdentRef;

		/// <summary>
		/// Constructor of class <see cref="OutputData"/>
		/// </summary>
		/// <param name="referenceContext">Context of data</param>
		/// <param name="anchorOf">Examined data</param>
		/// <param name="nerClass"><see cref="NerClasses"/> class of <see cref="AnchorOf"/></param>
		/// <param name="beginIndex">Index of beginning of <see cref="AnchorOf"/> inside of <see cref="ReferenceContext"/></param>
		/// <param name="endIndex">Index of end of <see cref="AnchorOf"/> inside of <see cref="ReferenceContext"/></param>
		/// <param name="inDBpedia">Determines whether <see cref="AnchorOf"/> has references in DBPedia. Default: false.</param>
		public OutputData(string referenceContext, string anchorOf, NerClasses nerClass, int beginIndex, int endIndex, bool inDBpedia = false)
		{
			ReferenceContext = referenceContext ?? throw new ArgumentNullException(nameof(referenceContext));
			AnchorOf = anchorOf ?? throw new ArgumentNullException(nameof(anchorOf));
			BeginIndex = beginIndex;
			EndIndex = endIndex;
			InDBpedia = inDBpedia;
			NerClass = nerClass;
		}

		/// <summary>
		/// Examined data
		/// </summary>
		public string AnchorOf { get; }

		/// <summary>
		/// <see cref="NerClasses"/> class of <see cref="AnchorOf"/>
		/// </summary>
		public NerClasses NerClass { get; }

		/// <summary>
		/// Index of beginning of <see cref="AnchorOf"/> inside of <see cref="ReferenceContext"/>
		/// </summary>
		private int BeginIndex { get; }

		/// <summary>
		/// Index of end of <see cref="AnchorOf"/> inside of <see cref="ReferenceContext"/>
		/// </summary>
		private int EndIndex { get; }

		/// <summary>
		/// Determines whether <see cref="AnchorOf"/> has references in DBPedia. Default: false.
		/// </summary>
		private bool InDBpedia { get; set; }

		/// <summary>
		/// Context of data
		/// </summary>
		private string ReferenceContext { get; }

		/// <summary>
		/// Public access to DBPedia reference
		/// </summary>
		public string TaIdentRef
		{
			private get => _taIdentRef;
			set
			{
				_taIdentRef = value;
				InDBpedia = true;
			}
		}

		/// <summary>
		/// Creating a rdf formatted output data
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			var output = string.Empty;
			var noCharAddress = ReferenceContext.Replace(">", string.Empty).Split('#')[0];

			output += $"<{noCharAddress}#char={BeginIndex},{EndIndex}>" + Environment.NewLine;
			output += Insert("\t") + "a" + Insert("\t", 6) + "nif:RFC5147String, nif:String;" + Environment.NewLine;
			output += Insert("\t") + "nif:anchorOf" + Insert("\t", 3) + $"\"{AnchorOf}\"@en;" + Environment.NewLine;
			output += Insert("\t") + "nif:beginIndex" + Insert("\t", 3) + $"\"{BeginIndex}\"^^xsd:nonNegativeInteger;" + Environment.NewLine;
			output += Insert("\t") + "nif:endIndex" + Insert("\t", 3) + $"\"{EndIndex}\"^^xsd:nonNegativeInteger; " + Environment.NewLine;
			output += Insert("\t") + "nif:referenceContext" + Insert("\t") + $"<{ReferenceContext}>" + Environment.NewLine;
			output += Insert("\t") + "itsrdf:taIdentRef" + Insert("\t", 2) + (InDBpedia ? $"dbpedia:{TaIdentRef.Split('/').Last()}" : $"<http://aksw.org/notInWiki/{AnchorOf.Replace(' ', '_')}>") + "." + Environment.NewLine;
			output += Environment.NewLine;
			return output;
		}

		/// <summary>
		/// Auxiliary method for multiple insertion of string
		/// </summary>
		/// <param name="s">What to duplicate</param>
		/// <param name="n">How many times to duplicate. Default: 1</param>
		/// <returns>Compound word of <see cref="s"/> duplicated <see cref="n"/> times</returns>
		private static string Insert(string s, int n = 1)
		{
			var output = string.Empty;

			for (var i = 0; i < n; i++) output += s;

			return output;
		}
	}
}