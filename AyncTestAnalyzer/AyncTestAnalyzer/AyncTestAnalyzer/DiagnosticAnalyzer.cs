using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace AyncTestAnalyzer
{
	[DiagnosticAnalyzer(LanguageNames.CSharp)]
	public class AyncTestAnalyzerAnalyzer : DiagnosticAnalyzer
	{
		public const string DiagnosticId = "ATA001";

		// You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
		internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
		internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.AnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
		internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
		internal const string Category = "Design";

		internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

		public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

		public override void Initialize(AnalysisContext context)
		{
			context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.Method);
		}

		private static void AnalyzeSymbol(SymbolAnalysisContext context)
		{
			var namedTypeSymbol = (IMethodSymbol)context.Symbol;

			var methodSymbol = (IMethodSymbol)context.Symbol;
			if (methodSymbol.IsAsync
				&& methodSymbol.ReturnsVoid
				&& methodSymbol.IsDefinition)
			{
				var diagnostic = Diagnostic.Create(Rule, methodSymbol.Locations[0], methodSymbol.Name);
				context.ReportDiagnostic(diagnostic);
			}
		}
	}
}
