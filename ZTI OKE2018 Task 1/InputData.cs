﻿using System;
using System.Collections.Generic;
using edu.stanford.nlp.ie.crf;

namespace ZTI_OKE2018_Task_1
{
	/// <summary>
	///     Main class of input data
	/// </summary>
	public class InputData
	{
		/// <summary>
		///     Constructor of <see cref="InputData" />
		/// </summary>
		/// <param name="text">Examined string</param>
		/// <param name="address">Which part of input file <see cref="IsString" /> belongs.</param>
		/// <param name="crfClassifier">NER Classifier of class <see cref="CRFClassifier" /></param>
		public InputData(string text, string address, CRFClassifier crfClassifier)
		{
			IsString = text ?? throw new ArgumentNullException(nameof(text));
			Address = address ?? throw new ArgumentNullException(nameof(address));
			CrfClassifier = crfClassifier ?? throw new ArgumentNullException(nameof(crfClassifier));
		}

		/// <summary>
		///     Examined string
		/// </summary>
		private string IsString { get; }

		/// <summary>
		///     Which part of input file <see cref="text" /> belongs.
		/// </summary>
		private string Address { get; }

		/// <summary>
		///     NER Classifier of class <see cref="CRFClassifier" />
		/// </summary>
		private CRFClassifier CrfClassifier { get; }

		/// <summary>
		///     Creating output data formatted to rdf.
		/// </summary>
		/// <param name="nc"><see cref="NerClasses" /> class</param>
		/// <returns>List of <see cref="NerClasses" categorized elements /></returns>
		public IEnumerable<OutputData> GetOntologyEntries(NerClasses nc)
		{
			var output = new List<OutputData>();

			var tagOpen = "<" + nc + ">";
			var tagClose = "</" + nc + ">";

			var i = 0;
			var j = 0;
			var k = 0;
			var count = 0;

			var nerText = Extensions.GetNerText(CrfClassifier, IsString);

			var occurrences = Extensions.CountStringOccurrences(nerText, tagOpen);

			while (count < occurrences)
			{
				var posA = nerText.IndexOf(tagOpen, i, StringComparison.Ordinal);
				var posB = nerText.IndexOf(tagClose, j, StringComparison.Ordinal);

				var adjustedPosA = posA + tagOpen.Length;

				var nazwa = string.Empty;

				if (posA < 0 || posB < 0) continue;

				i = posA + tagOpen.Length;
				j = posB + tagClose.Length;

				nazwa = nerText.Substring(adjustedPosA, posB - adjustedPosA);

				var t = new OutputData(Address, nazwa, nc, IsString.IndexOf(nazwa, k, StringComparison.Ordinal), IsString.IndexOf(nazwa, k, StringComparison.Ordinal) + nazwa.Length - 1);
				output.Add(t);

				k = IsString.IndexOf(nazwa, k, StringComparison.Ordinal) + nazwa.Length;
				count += 1;
			}

			return output;
		}
	}
}