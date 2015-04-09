using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Composition;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CodeActions;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Rename;
using Microsoft.CodeAnalysis.Text;

namespace AyncTestAnalyzer
{
	[ExportCodeFixProvider(LanguageNames.CSharp, Name = nameof(AyncTestAnalyzerCodeFixProvider)), Shared]
	public class AyncTestAnalyzerCodeFixProvider : CodeFixProvider
	{
		public sealed override ImmutableArray<string> FixableDiagnosticIds
		{
			get { return ImmutableArray.Create(AyncTestAnalyzerAnalyzer.DiagnosticId); }
		}

		public sealed override FixAllProvider GetFixAllProvider()
		{
			return WellKnownFixAllProviders.BatchFixer;
		}

		public sealed override async Task RegisterCodeFixesAsync(CodeFixContext context)
		{
			var root = await context.Document.GetSyntaxRootAsync(context.CancellationToken).ConfigureAwait(false);

			// TODO: Replace the following code with your own analysis, generating a CodeAction for each fix to suggest
			var diagnostic = context.Diagnostics.First();
			var diagnosticSpan = diagnostic.Location.SourceSpan;

			// Find the type declaration identified by the diagnostic.
			var declaration = root.FindToken(diagnosticSpan.Start).Parent.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().First();

			// Register a code action that will invoke the fix.
			context.RegisterCodeFix(
				CodeAction.Create("Change return type to Task", c => MakeUppercaseAsync(context.Document, declaration, c)),
				diagnostic);
		}

		private async Task<Document> MakeUppercaseAsync(Document document, MethodDeclarationSyntax methodDecl, CancellationToken cancellationToken)
		{
			var semanticModel = await document.GetSemanticModelAsync(cancellationToken);
			var newTypeSyntax = SyntaxFactory.ParseTypeName("Task")
				.WithLeadingTrivia(methodDecl.ReturnType.GetLeadingTrivia())
				.WithTrailingTrivia(methodDecl.ReturnType.GetTrailingTrivia());

			var result = methodDecl.WithReturnType(newTypeSyntax);

			var root = await document.GetSyntaxRootAsync();
			var newRoot = root.ReplaceNode(methodDecl, result);
			var newDocument = document.WithSyntaxRoot(newRoot);
			return newDocument;
		}
	}
}