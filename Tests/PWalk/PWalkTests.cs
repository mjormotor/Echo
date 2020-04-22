using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Echo.PWalkService.Tests
{
	[TestClass]
	public class PWalkTests
	{
		[TestMethod]
		public void TargetTypeTest()
		{
			var root = new Root();

			PWalk.TargetType<Target1>(_ => _.Invoke(), root, 1);

			Assert.IsTrue(root.WalkPathClass.Target.InvokedCount == 1);
			Assert.IsTrue(root.WalkPathProperty.Target.InvokedCount == 1);
			Assert.IsTrue(root.WalkSkipProperty.Target.InvokedCount == 0);
		}

		[TestMethod]
		public void MarkedWithTest()
		{
			var root = new Root();
			
			Action<object, PWalkContext> action = (_, context) =>
			{
				var type = context.CurrentNode.AssignedType;
				if (type != null)
				{
					if (type == typeof(string))
					{
						var stringMark = (StringMarkAttribute)context.CurrentMark;
						context.CurrentNode.Value = stringMark.Text;
					}
				}
			};

			PWalk.MarkedWith<StringMarkAttribute>(action, root);

			Assert.IsTrue(root.StringHolder.Text == Target2.VisitedText);
		}

		[TestMethod]
		public void InvokeMarkedFunctionTest()
		{
			var root = new Root();

			PWalk.InvokeMarkedWith<FuncMarkAttribute>(root, new object[] { PWalkProxyParameter.Mark, });

			Assert.IsTrue(root.FuncHolder.Text == Target3.VisitedText);
		}

		[TestMethod]
		public void InvokeMarkedEventTest()
		{
			var root = new Root();

			const string VisitedText = "visited";
			var invokeCount = 0;
			EventHandler<ItemEventArgs<string>> handler = (sender, e) =>
			{
				++invokeCount;
				Assert.IsTrue(e.Item == VisitedText);
			};

			root.FuncHolder.Visited += handler;
			PWalk.InvokeMarkedWith<EventMarkAttribute>(root, new object[] { PWalkProxyParameter.Current, new ItemEventArgs<string>(VisitedText) });
			root.FuncHolder.Visited -= handler;

			Assert.IsTrue(invokeCount == 1);
		}

		[TestMethod]
		public void CodeAnalysisTest()
		{
			var trace = new System.Diagnostics.StackTrace(true);
			var frame = trace.GetFrame(0);
			var filePath = frame.GetFileName();
			var directoryPath = System.IO.Path.GetDirectoryName(filePath);
			var targetDirectoryPath = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(System.IO.Path.GetDirectoryName(directoryPath)), "PWalk");
			var targetFilePath = System.IO.Path.Combine(targetDirectoryPath, "TestManager.cs");
			if (!System.IO.File.Exists(targetFilePath))
			{
				return;
			}

			var text = System.IO.File.ReadAllText(targetFilePath);
			var tree = CSharpSyntaxTree.ParseText(text, null, targetFilePath);
			var managerCode = GenerateManagerCode(tree);
			if (!string.IsNullOrEmpty(managerCode))
			{
				var code = GenerateCode(tree);
			}

			#region method GenerateManagerCode
			string GenerateManagerCode(SyntaxTree tree)
			{
				string ret = null;
				if (tree != null)
				{
					var root = tree.GetRoot() as CompilationUnitSyntax;
					if (root != null)
					{
						var generator = new ManagerCodeGenerator();
						var result = generator.Visit(tree.GetRoot());
						ret = result.ToString();
					}
				}

				return ret;
			}
			#endregion // method GenerateManagerCode

			#region method GenerateCode
			string GenerateCode(SyntaxTree tree)
			{
				string ret = null;
				if (tree != null)
				{
					var root = tree.GetRoot() as CompilationUnitSyntax;
					if (root != null)
					{
						var generator = new CodeGenerator();
						var result = generator.Visit(tree.GetRoot());
						ret = result.ToString();
					}
				}

				return ret;
			}
			#endregion // method GenerateCode
		}

		#region private members
		#region class Root
		private class Root
		{
			public Target1Shell WalkPathClass { get; } = new Target1Shell();

			[PWalkPath(typeof(Target1))]
			public Target1Holder WalkPathProperty { get; } = new Target1Holder();

			public Target1Holder WalkSkipProperty { get; } = new Target1Holder();

			public Target2 StringHolder { get; } = new Target2();

			public Target3 FuncHolder { get; } = new Target3();
		}
		#endregion // class Root

		#region class Target1
		private class Target1
		{
			public int InvokedCount => this.invokedCount;

			public void Invoke()
			{
				++this.invokedCount;
			}

			#region private members
			private int invokedCount;
			#endregion // private members
		}
		#endregion // class Target1

		#region class Target2
		private class Target2
		{
			public const string VisitedText = "visited";

			[StringMark(VisitedText)]
			public string Text { get; set; }
		}
		#endregion // class Target2

		#region class Target3
		private class Target3
		{
			public const string VisitedText = "visited";

			[EventMark]
			public event EventHandler<ItemEventArgs<string>> Visited;

			public string Text => this.text;

			[FuncMark(VisitedText)]
			public void Func(FuncMarkAttribute mark)
			{
				this.text = mark.Text;
			}

			public void RaiseVisited(string text)
			{
				Visited?.Invoke(this, new ItemEventArgs<string>(text));
			}

			#region private members
			private string text;
			#endregion // private members
		}
		#endregion // class Target3

		#region class Target1Shell
		[PWalkPath(typeof(Target1))]
		private class Target1Shell
		{
			public Target1 Target { get; } = new Target1();
		}
		#endregion // class Target1Shell

		#region class Target1Holder
		private class Target1Holder
		{
			public Target1 Target { get; } = new Target1();
		}
		#endregion // class Target1Holder

		#region class StringMarkAttribute
		private class StringMarkAttribute : PWalkMarkAttribute
		{
			public StringMarkAttribute(string text)
			{
				Text = text;
			}

			public string Text { get; }
		}
		#endregion // class StringMarkAttribute

		#region class FuncMarkAttribute
		private class FuncMarkAttribute : PWalkMarkAttribute
		{
			public FuncMarkAttribute(string text)
			{
				Text = text;
			}

			public string Text { get; }
		}
		#endregion // class FuncMarkAttribute

		#region class EventMarkAttribute
		private class EventMarkAttribute : PWalkMarkAttribute
		{
		}
		#endregion // class EventMarkAttribute

		#region class ManagerCodeGenerator
		private class ManagerCodeGenerator : CSharpSyntaxRewriter
		{
			#region CSharpSyntaxRewriter virtual member override
			public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
			{
				var ret = node;
				var members = new List<MemberDeclarationSyntax>();
				foreach (var member in node.Members)
				{
					var kind = member.Kind();
					switch (kind)
					{
						case SyntaxKind.MethodDeclaration:
							var method = (MethodDeclarationSyntax)member;
							if (method.Modifiers.Any(_ => _.Kind() == SyntaxKind.PublicKeyword))
							{
								var name = method.Identifier.Text;
								switch (name)
								{
									case nameof(TestManager.EveryNodeIf):
										//members.AddRange(VaryEveryNode(method));
										break;

									case nameof(TestManager.InvokeMarkedWith):
										members.AddRange(VaryInvokeMarkedWith(method));
										break;

									case nameof(TestManager.MarkedWith):
										members.AddRange(VaryMarkedWith(method));
										break;

									case nameof(TestManager.TargetType):
										members.AddRange(VaryTargetType(method));
										break;
								}
							}
							break;
					}
				}

				if (members.Count > 0)
				{
					members[0] = members[0].WithLeadingTrivia(SyntaxFactory.TriviaList(members[0].GetLeadingTrivia().Skip(1)));
				}

				if (ret.HasLeadingTrivia)
				{
					// removing document comment of class declaration
					var trivia = ret.GetLeadingTrivia().First();
					var kind = trivia.Kind();
					switch (kind)
					{
						case SyntaxKind.WhitespaceTrivia:
							// remaining indent
							ret = ret.WithLeadingTrivia(SyntaxFactory.TriviaList(trivia));
							break;

						default:
							ret = ret.WithoutLeadingTrivia();
							break;
					}
				}

				return ret
					.WithMembers(new SyntaxList<MemberDeclarationSyntax>(members))
					.WithCloseBraceToken(ret.CloseBraceToken.WithoutTrivia());

				#region method EvaluateIndent
				string EvaluateIndent(MethodDeclarationSyntax method)
				{
					if (method.Body != null)
					{
						var match = IndentPattern.Match(method.Body.ToString());
						if (match != null)
						{
							return match.Groups[1].Value;
						}
					}

					if (method.ExpressionBody != null)
					{
						var match = IndentPattern.Match(method.ToString());
						if (match != null)
						{
							return match.Groups[1].Value;
						}
					}

					return "\t";
				}
				#endregion // method EvaluateIndent

				#region method EvaluateDocumentCommentElementLineFeed
				string EvaluateDocumentCommentElementLineFeed(XmlNodeSyntax node)
				{
					var match = DocumentCommentElementLineFeedPattern.Match(node.ToString());
					if (match == null)
					{
						return "\r\n/// ";
					}

					return match.Value;
				}
				#endregion // method EvaluateDocumentCommentElementLineFeed

				#region method RewriteExpressionBodyCore
				MethodDeclarationSyntax RewriteExpressionBodyCore(MethodDeclarationSyntax method, string expression)
				{
					var indent = EvaluateIndent(method);
					return method.WithBody(null)
							.WithExpressionBody(
								SyntaxFactory.ArrowExpressionClause(
									SyntaxFactory.Token(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(indent)), SyntaxKind.EqualsGreaterThanToken, SyntaxFactory.TriviaList(SyntaxFactory.Whitespace(" "))),
									SyntaxFactory.ParseExpression(expression)))
							.WithSemicolonToken(SyntaxFactory.Token(SyntaxKind.SemicolonToken))
							.WithTrailingTrivia(SyntaxFactory.TriviaList(SyntaxFactory.Whitespace("\r\n")));
				}
				#endregion // method RewriteExpressionBodyCore

				#region method VaryParameterListCore
				IEnumerable<MethodDeclarationSyntax> VaryParameterListCore(MethodDeclarationSyntax method, string expression)
				{
					yield return RemoveUserDataAndStep(method, expression);
					yield return RemoveStep(method, expression);
					yield return RemoveUserData(method, expression);
					yield return method;

					#region method RemoveStep
					MethodDeclarationSyntax RemoveStep(MethodDeclarationSyntax method, string expression)
					{
						var ret = RewriteLeadingTrivia(method);
						ret = RewriteParameterList(ret);
						ret = RewriteBody(ret, expression.Replace("step", "PWalk.InfiniteStep"));
						return ret;

						#region method RewriteLeadingTrivia
						MethodDeclarationSyntax RewriteLeadingTrivia(MethodDeclarationSyntax method)
						{
							var leadingTrivia = new List<SyntaxTrivia>();
							foreach (var current in method.GetLeadingTrivia())
							{
								var trivia = current;
								var kind = trivia.Kind();
								switch (kind)
								{
									case SyntaxKind.SingleLineDocumentationCommentTrivia:
										trivia = RewriteComment(trivia);
										break;
								}

								leadingTrivia.Add(trivia);
							}

							return method.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia));

							#region method RewriteComment
							SyntaxTrivia RewriteComment(SyntaxTrivia trivia)
							{
								var comment = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

								var nodes = new List<XmlNodeSyntax>(comment.Content.Count);
								foreach (var current in comment.Content)
								{
									var node = current;
									var kind = node.Kind();
									switch (kind)
									{
										case SyntaxKind.XmlElement:
											var element = (XmlElementSyntax)node;
											var name = element.StartTag.Name;
											switch (name.ToString())
											{
												case "param":
													var paramName = element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First(_ => _.Name.ToString() == "name");
													switch (paramName.Identifier.GetText().ToString())
													{
														// removing line of param of step
														case "step":
															nodes.RemoveAt(nodes.Count - 1);
															node = null;
															break;
													}
													break;

												default:
													break;
											}
											break;
									}

									if (node != null)
									{
										nodes.Add(node);
									}
								}

								return SyntaxFactory.Trivia(comment.WithContent(SyntaxFactory.List(nodes)));
							}
							#endregion // method RewriteComment
						}
						#endregion // method RewriteLeadingTrivia

						#region method RewriteParameterList
						MethodDeclarationSyntax RewriteParameterList(MethodDeclarationSyntax method)
						{
							var step = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "step");
							return method.WithParameterList(method.ParameterList.RemoveNode(step, SyntaxRemoveOptions.KeepNoTrivia));
						}
						#endregion // method RewriteParameterList

						#region method RewriteBody
						MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method, string expression)
						{
							return RewriteExpressionBodyCore(method, expression.Replace("step", "PWalk.InfiniteStep"));
						}
						#endregion // method RewriteBody
					}
					#endregion // method RemoveUserData

					#region method RemoveUserData
					MethodDeclarationSyntax RemoveUserData(MethodDeclarationSyntax method, string expression)
					{
						var ret = RewriteLeadingTrivia(method);
						ret = RewriteParameterList(ret);
						ret = RewriteBody(ret, expression);
						return ret;

						#region method RewriteLeadingTrivia
						MethodDeclarationSyntax RewriteLeadingTrivia(MethodDeclarationSyntax method)
						{
							var leadingTrivia = new List<SyntaxTrivia>();
							foreach (var current in method.GetLeadingTrivia())
							{
								var trivia = current;
								var kind = trivia.Kind();
								switch (kind)
								{
									case SyntaxKind.SingleLineDocumentationCommentTrivia:
										trivia = RewriteComment(trivia);
										break;
								}

								leadingTrivia.Add(trivia);
							}

							return method.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia));

							#region method RewriteComment
							SyntaxTrivia RewriteComment(SyntaxTrivia trivia)
							{
								var comment = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

								var nodes = new List<XmlNodeSyntax>(comment.Content.Count);
								foreach (var current in comment.Content)
								{
									var node = current;
									var kind = node.Kind();
									switch (kind)
									{
										case SyntaxKind.XmlElement:
											var element = (XmlElementSyntax)node;
											var name = element.StartTag.Name;
											switch (name.ToString())
											{
												case "param":
													var paramName = element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First(_ => _.Name.ToString() == "name");
													switch (paramName.Identifier.GetText().ToString())
													{
														// removing line of param of userData
														case "userData":
															nodes.RemoveAt(nodes.Count - 1);
															node = null;
															break;
													}
													break;

												default:
													break;
											}
											break;
									}

									if (node != null)
									{
										nodes.Add(node);
									}
								}

								return SyntaxFactory.Trivia(comment.WithContent(SyntaxFactory.List(nodes)));
							}
							#endregion // method RewriteComment
						}
						#endregion // method RewriteLeadingTrivia

						#region method RewriteParameterList
						MethodDeclarationSyntax RewriteParameterList(MethodDeclarationSyntax method)
						{
							var userData = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "userData");
							return method.WithParameterList(method.ParameterList.RemoveNode(userData, SyntaxRemoveOptions.KeepNoTrivia));
						}
						#endregion // method RewriteParameterList

						#region method RewriteBody
						MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method, string expression)
						{
							return RewriteExpressionBodyCore(method, expression.Replace("userData", "null"));
						}
						#endregion // method RewriteBody
					}
					#endregion // method RemoveUserData

					#region method RemoveUserDataAndStep
					MethodDeclarationSyntax RemoveUserDataAndStep(MethodDeclarationSyntax method, string expression)
					{
						var ret = RewriteLeadingTrivia(method);
						ret = RewriteParameterList(ret);
						ret = RewriteBody(ret, expression);
						return ret;

						#region method RewriteLeadingTrivia
						MethodDeclarationSyntax RewriteLeadingTrivia(MethodDeclarationSyntax method)
						{
							var leadingTrivia = new List<SyntaxTrivia>();
							foreach (var current in method.GetLeadingTrivia())
							{
								var trivia = current;
								var kind = trivia.Kind();
								switch (kind)
								{
									case SyntaxKind.SingleLineDocumentationCommentTrivia:
										trivia = RewriteComment(trivia);
										break;
								}

								leadingTrivia.Add(trivia);
							}

							return method.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia));

							#region method RewriteComment
							SyntaxTrivia RewriteComment(SyntaxTrivia trivia)
							{
								var comment = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

								var nodes = new List<XmlNodeSyntax>(comment.Content.Count);
								foreach (var current in comment.Content)
								{
									var node = current;
									var kind = node.Kind();
									switch (kind)
									{
										case SyntaxKind.XmlElement:
											var element = (XmlElementSyntax)node;
											var name = element.StartTag.Name;
											switch (name.ToString())
											{
												case "param":
													var paramName = element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First(_ => _.Name.ToString() == "name");
													switch (paramName.Identifier.GetText().ToString())
													{
														// removing line of param of step and userData
														case "step":
														case "userData":
															nodes.RemoveAt(nodes.Count - 1);
															node = null;
															break;
													}
													break;

												default:
													break;
											}
											break;
									}

									if (node != null)
									{
										nodes.Add(node);
									}
								}

								return SyntaxFactory.Trivia(comment.WithContent(SyntaxFactory.List(nodes)));
							}
							#endregion // method RewriteComment
						}
						#endregion // method RewriteLeadingTrivia

						#region method RewriteParameterList
						MethodDeclarationSyntax RewriteParameterList(MethodDeclarationSyntax method)
						{
							var userData = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "userData");
							var step = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "step");
							return method.WithParameterList(method.ParameterList.RemoveNodes(userData.EnumerateWith(step), SyntaxRemoveOptions.KeepNoTrivia));
						}
						#endregion // method RewriteParameterList

						#region method RewriteBody
						MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method, string expression)
						{
							return RewriteExpressionBodyCore(method, expression.Replace("userData, step", "null"));
						}
						#endregion // method RewriteBody
					}
					#endregion // method RemoveUserDataAndStep
				}
				#endregion // method VaryParameterListCore

				#region method VaryEveryNode
				IEnumerable<MethodDeclarationSyntax> VaryEveryNode(MethodDeclarationSyntax method)
				{
					foreach (var ret in GenerateEveryNode(method).Align(VaryProcessParameters).Align(VaryEveryNodeParameterList))
					{
						yield return ret;
					}

					MethodDeclarationSyntax last = null;
					foreach (var ret in method.Align(VaryConditionAndProcessParameters).Align(VaryEveryNodeIfParameterList))
					{
						if (last != null)
						{
							yield return last;
						}

						last = ret;
					}

					#region method GenerateEveryNode
					MethodDeclarationSyntax GenerateEveryNode(MethodDeclarationSyntax method)
					{
						var ret = method.WithIdentifier(SyntaxFactory.Identifier("EveryNode"));
						ret = RewriteLeadingTrivia(ret);
						ret = RewriteParameterList(ret);
						ret = RewriteBody(ret);
						return ret;

						#region method RewriteLeadingTrivia
						MethodDeclarationSyntax RewriteLeadingTrivia(MethodDeclarationSyntax method)
						{
							var leadingTrivia = new List<SyntaxTrivia>();
							foreach (var current in method.GetLeadingTrivia())
							{
								var trivia = current;
								var kind = trivia.Kind();
								switch (kind)
								{
									case SyntaxKind.SingleLineDocumentationCommentTrivia:
										trivia = RewriteComment(trivia);
										break;
								}

								leadingTrivia.Add(trivia);
							}

							return method.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia));

							#region method RewriteComment
							SyntaxTrivia RewriteComment(SyntaxTrivia trivia)
							{
								var comment = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

								var nodes = new List<XmlNodeSyntax>(comment.Content.Count);
								foreach (var current in comment.Content)
								{
									var node = current;
									var kind = node.Kind();
									switch (kind)
									{
										case SyntaxKind.XmlElement:
											var element = (XmlElementSyntax)node;
											var name = element.StartTag.Name;
											switch (name.ToString())
											{
												case "param":
													var paramName = element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First(_ => _.Name.ToString() == "name");
													switch (paramName.Identifier.GetText().ToString())
													{
														// removing line of param of condition 
														case "condition":
															nodes.RemoveAt(nodes.Count - 1);
															node = null;
															break;
													}
													break;

												case "remarks":
													// rewriting remarks text
													var remarks = element.Content.First();
													node = element.ReplaceNode(remarks, SyntaxFactory.XmlText("\r\n探索するノードごとに処理を実行します。\r\n".Replace("\r\n", EvaluateDocumentCommentElementLineFeed(remarks))));
													break;

												case "summary":
													// rewriting summary text
													var summary = element.Content.First();
													node = element.ReplaceNode(summary, SyntaxFactory.XmlText("\r\nノード毎処理の実行\r\n".Replace("\r\n", EvaluateDocumentCommentElementLineFeed(summary))));
													break;

												default:
													break;
											}
											break;
									}

									if (node != null)
									{
										nodes.Add(node);
									}
								}

								return SyntaxFactory.Trivia(comment.WithContent(SyntaxFactory.List(nodes)));
							}
							#endregion // method RewriteComment
						}
						#endregion // method RewriteLeadingTrivia

						#region method RewriteParameterList
						MethodDeclarationSyntax RewriteParameterList(MethodDeclarationSyntax method)
						{
							var condition = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "condition");
							return method.WithParameterList(method.ParameterList.RemoveNode(condition, SyntaxRemoveOptions.KeepNoTrivia));
						}
						#endregion // method RewriteParameterList

						#region method RewriteBody
						MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
						{
							return RewriteExpressionBodyCore(method, "EveryNodeIf(() => true, process, subject, userData, step, options)");
						}
						#endregion // method RewriteBody
					}
					#endregion // method GenerateEveryNode

					#region method VaryConditionAndProcessParameters
					IEnumerable<MethodDeclarationSyntax> VaryConditionAndProcessParameters(MethodDeclarationSyntax method)
					{
						foreach (var ret in GenerateNonparametricConditionParameters(method).Align(VaryProcessParameters))
						{
							yield return ret;
						}

						foreach (var ret in GenerateSubjectiveConditionParameters(method).Align(VaryProcessParameters))
						{
							yield return ret;
						}

						foreach (var ret in method.Align(VaryProcessParameters))
						{
							yield return ret;
						}

						#region method GenerateNonparametricConditionParameters
						MethodDeclarationSyntax GenerateNonparametricConditionParameters(MethodDeclarationSyntax method)
						{
							var condition = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "condition");
							var ret = method.WithParameterList(method.ParameterList.ReplaceNode(condition, condition.WithType(SyntaxFactory.ParseTypeName("Func<bool> "))));
							ret = RewriteBody(ret);
							return ret;

							#region method RewriteBody
							MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
							{
								return RewriteExpressionBodyCore(method, "EveryNodeIf((current, context) => condition(), process, subject, userData, step, options)");
							}
							#endregion // method RewriteBody
						}
						#endregion // method GenerateNonparametricConditionParameters

						#region method GenerateSubjectiveConditionParameters
						MethodDeclarationSyntax GenerateSubjectiveConditionParameters(MethodDeclarationSyntax method)
						{
							var condition = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "condition");
							var ret = method.WithParameterList(method.ParameterList.ReplaceNode(condition, condition.WithType(SyntaxFactory.ParseTypeName("Func<object, bool> "))));
							ret = RewriteBody(ret);
							return ret;

							#region method RewriteBody
							MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
							{
								return RewriteExpressionBodyCore(method, "EveryNodeIf((current, context) => condition(current), process, subject, userData, step, options)");
							}
							#endregion // method RewriteBody
						}
						#endregion // method GenerateSubjectiveConditionParameters

						#region method VaryProcessParameters
						IEnumerable<MethodDeclarationSyntax> VaryProcessParameters(MethodDeclarationSyntax method)
						{
							yield return GenerateSubjectiveProcessParameters(method);
							yield return method;

							#region method GenerateSubjectiveProcessParameters
							MethodDeclarationSyntax GenerateSubjectiveProcessParameters(MethodDeclarationSyntax method)
							{
								var process = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "process");
								var ret = method.WithParameterList(method.ParameterList.ReplaceNode(process, process.WithType(SyntaxFactory.ParseTypeName("Action<object> "))));
								ret = RewriteBody(ret);

								return ret;

								#region method RewriteBody
								MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
								{
									return RewriteExpressionBodyCore(method, "EveryNodeIf(condition, (current, context) => process(current), subject, userData, step, options)");
								}
								#endregion // method RewriteBody
							}
							#endregion // method GenerateSubjectiveProcessParameters
						}
						#endregion // method VaryProcessParameters
					}
					#endregion // method VaryConditionAndProcessParameters

					#region method VaryProcessParameters
					IEnumerable<MethodDeclarationSyntax> VaryProcessParameters(MethodDeclarationSyntax method)
					{
						yield return GenerateSubjectiveProcessParameters(method);
						yield return method;

						#region method GenerateSubjectiveProcessParameters
						MethodDeclarationSyntax GenerateSubjectiveProcessParameters(MethodDeclarationSyntax method)
						{
							var process = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "process");
							var ret = method.WithParameterList(method.ParameterList.ReplaceNode(process, process.WithType(SyntaxFactory.ParseTypeName("Action<object> "))));
							ret = RewriteBody(ret);
							return ret;

							#region method RewriteBody
							MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
							{
								return RewriteExpressionBodyCore(method, "EveryNode((current, context) => process(current), subject, userData, step, options)");
							}
							#endregion // method RewriteBody
						}
						#endregion // method GenerateSubjectiveProcessParameters
					}
					#endregion // method VaryProcessParameters

					#region method VaryEveryNodeParameterList
					IEnumerable<MethodDeclarationSyntax> VaryEveryNodeParameterList(MethodDeclarationSyntax method)
					{
						return VaryParameterListCore(method, "EveryNode(process, subject, userData, step, options)");
					}
					#endregion // method VaryEveryNodeParameterList

					#region method VaryEveryNodeIfParameterList
					IEnumerable<MethodDeclarationSyntax> VaryEveryNodeIfParameterList(MethodDeclarationSyntax method)
					{
						return VaryParameterListCore(method, "EveryNodeIf(condition, process, subject, userData, step, options)");
					}
					#endregion // method VaryEveryNodeIfParameterList
				}
				#endregion // method VaryEveryNode

				#region method VaryInvokeMarkedWith
				IEnumerable<MethodDeclarationSyntax> VaryInvokeMarkedWith(MethodDeclarationSyntax method)
				{
					yield return method;
				}
				#endregion // method VaryInvokeMarkedWith

				#region method VaryMarkedWith
				IEnumerable<MethodDeclarationSyntax> VaryMarkedWith(MethodDeclarationSyntax method)
				{
					yield return method;
				}
				#endregion // method VaryMarkedWith

				#region method VaryTargetType
				IEnumerable<MethodDeclarationSyntax> VaryTargetType(MethodDeclarationSyntax method)
				{
					foreach ((var generated, var index) in method.Align(VaryTypeParameters).Select((_, index) =>(_, index)))
					{
						if (index == 0)
						{
							MethodDeclarationSyntax last = null;
							foreach (var ret in generated.Align(VaryProcessParameters))
							{
								if (last != null)
								{
									yield return last;
								}

								last = ret;
							}
						}
						else
						{
							foreach (var ret in generated.Align(VaryProcessParameters))
							{
								yield return ret;
							}
						}
					}

					#region method VaryTypeParameters
					IEnumerable<MethodDeclarationSyntax> VaryTypeParameters(MethodDeclarationSyntax method)
					{
						yield return method;

						var ret = GenerateInitial(method);
						for (var index = 2; index <= 4; ++index)
						{
							ret = AddTypeParameter(ret, index);
							yield return ret;
						}

						#region method GenerateInitial
						MethodDeclarationSyntax GenerateInitial(MethodDeclarationSyntax method)
						{
							var ret = RewriteLeadingTrivia(method);
							ret = RewriteTypeParameterList(ret);
							ret = RewriteParameterList(ret);
							ret = RewriteBody(ret);
							return ret;

							#region method RewriteLeadingTrivia
							MethodDeclarationSyntax RewriteLeadingTrivia(MethodDeclarationSyntax method)
							{
								var leadingTrivia = new List<SyntaxTrivia>();
								foreach (var current in method.GetLeadingTrivia())
								{
									var trivia = current;
									var kind = trivia.Kind();
									switch (kind)
									{
										case SyntaxKind.SingleLineDocumentationCommentTrivia:
											trivia = RewriteComment(trivia);
											break;
									}

									leadingTrivia.Add(trivia);
								}

								return method.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia));

								#region method RewriteComment
								SyntaxTrivia RewriteComment(SyntaxTrivia trivia)
								{
									var comment = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

									var nodes = new List<XmlNodeSyntax>(comment.Content.Count);
									foreach (var current in comment.Content)
									{
										var node = current;
										var kind = node.Kind();
										switch (kind)
										{
											case SyntaxKind.XmlElement:
												var element = (XmlElementSyntax)node;
												var name = element.StartTag.Name;
												switch (name.ToString())
												{
													case "param":
														var paramName = element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First(_ => _.Name.ToString() == "name");
														switch (paramName.Identifier.GetText().ToString())
														{
															// rewriting process text
															case "process":
																element = element.ReplaceNode(paramName, SyntaxFactory.XmlNameAttribute("process1"));
																var process = element.Content.First();
																node = element.ReplaceNode(process, SyntaxFactory.XmlText("型 1 への処理"));
																break;
														}
														break;

													default:
														break;
												}
												break;
										}

										if (node != null)
										{
											nodes.Add(node);
										}
									}

									return SyntaxFactory.Trivia(comment.WithContent(SyntaxFactory.List(nodes)));
								}
								#endregion // method RewriteComment
							}
							#endregion // method RewriteLeadingTrivia

							#region method RewriteTypeParameterList
							MethodDeclarationSyntax RewriteTypeParameterList(MethodDeclarationSyntax method)
							{
								return method.WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList(SyntaxFactory.TypeParameter("T1").Enumerate())));
							}
							#endregion // method RewriteTypeParameterList

							#region method RewriteTypeParameterList
							MethodDeclarationSyntax RewriteParameterList(MethodDeclarationSyntax method)
							{
								var process = method.ParameterList.Parameters.First(_ => _.Identifier.Text == "process");
								return method.WithParameterList(method.ParameterList.ReplaceNode(process, process.WithIdentifier(SyntaxFactory.Identifier("process1")).WithType(SyntaxFactory.ParseTypeName("Action<T1, PWalkContext> "))));
							}
							#endregion // method RewriteParameterList

							#region method RewriteBody
							MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
							{
								var statements = new List<StatementSyntax>(method.Body.Statements.Count);
								foreach (var current in method.Body.Statements)
								{
									var statement = current;
									var kind = statement.Kind();
									switch (kind)
									{
										case SyntaxKind.LocalDeclarationStatement:
										case SyntaxKind.IfStatement:
											statement = SyntaxFactory.ParseStatement(ProcessParameterPattern.Replace(statement.ToFullString(), "process1"));
											break;
									}

									statements.Add(statement);
								}

								return method.WithBody(method.Body.WithStatements(SyntaxFactory.List(statements)));
							}
							#endregion // method RewriteBody
						}
						#endregion // method GenerateInitial

						#region method AddTypeParameter
						MethodDeclarationSyntax AddTypeParameter(MethodDeclarationSyntax method, int index)
						{
							var addingParamName = $"process{index}";
							var targetParamName = $"process{index - 1}";
							var ret = RewriteLeadingTrivia(method);
							ret = RewriteTypeParameterList(ret);
							ret = RewriteParameterList(ret);
							ret = RewriteBody(ret);
							return ret;

							#region method RewriteLeadingTrivia
							MethodDeclarationSyntax RewriteLeadingTrivia(MethodDeclarationSyntax method)
							{
								var leadingTrivia = new List<SyntaxTrivia>();
								foreach (var current in method.GetLeadingTrivia())
								{
									var trivia = current;
									var kind = trivia.Kind();
									switch (kind)
									{
										case SyntaxKind.SingleLineDocumentationCommentTrivia:
											trivia = RewriteComment(trivia);
											break;
									}

									leadingTrivia.Add(trivia);
								}

								return method.WithLeadingTrivia(SyntaxFactory.TriviaList(leadingTrivia));

								#region method RewriteComment
								SyntaxTrivia RewriteComment(SyntaxTrivia trivia)
								{
									var comment = (DocumentationCommentTriviaSyntax)trivia.GetStructure();

									XmlNodeSyntax lastNode = null;
									foreach (var node in comment.Content)
									{
										var kind = node.Kind();
										switch (kind)
										{
											case SyntaxKind.XmlElement:
												var element = (XmlElementSyntax)node;
												var name = element.StartTag.Name;
												switch (name.ToString())
												{
													case "param":
														var paramName = element.StartTag.Attributes.OfType<XmlNameAttributeSyntax>().First(_ => _.Name.ToString() == "name");
														if (paramName.Identifier.GetText().ToString() == targetParamName)
														{
															// adding process text
															element = element.ReplaceNode(paramName, SyntaxFactory.XmlNameAttribute(addingParamName));
															var process = element.Content.First();
															return SyntaxFactory.Trivia(comment.InsertNodesAfter(node, lastNode.EnumerateWith(element.ReplaceNode(process, SyntaxFactory.XmlText($"型 {index} への処理")))));
														}
														break;
												}
												break;
										}

										lastNode = node;
									}

									return trivia;
								}
								#endregion // method RewriteComment
							}
							#endregion // method RewriteLeadingTrivia

							#region method RewriteTypeParameterList
							MethodDeclarationSyntax RewriteTypeParameterList(MethodDeclarationSyntax method)
							{
								//return method.WithTypeParameterList(SyntaxFactory.TypeParameterList(method.TypeParameterList.Parameters.Add(SyntaxFactory.TypeParameter($"T{index}").WithLeadingTrivia(SyntaxFactory.Whitespace(" ")))));
								return method.WithTypeParameterList(SyntaxFactory.TypeParameterList(method.TypeParameterList.Parameters.Add(SyntaxFactory.TypeParameter($" T{index}"))));
							}
							#endregion // method RewriteTypeParameterList

							#region method RewriteTypeParameterList
							MethodDeclarationSyntax RewriteParameterList(MethodDeclarationSyntax method)
							{
								var process = method.ParameterList.Parameters.First(_ => _.Identifier.Text == targetParamName);
								return method.WithParameterList(method.ParameterList.InsertNodesAfter(process, process.WithIdentifier(SyntaxFactory.Identifier(addingParamName)).WithType(SyntaxFactory.ParseTypeName($" Action<T{index}, PWalkContext> ")).Enumerate()));
							}
							#endregion // method RewriteParameterList

							#region method RewriteBody
							MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
							{
								var statements = new List<StatementSyntax>(method.Body.Statements.Count);
								foreach (var current in method.Body.Statements)
								{
									var statement = current;
									var kind = statement.Kind();
									switch (kind)
									{
										case SyntaxKind.LocalDeclarationStatement:
											{
												var text = statement.ToFullString();
												var lines = new List<string>(text.Split("\r\n"));
												if (lines.Count >= 3)
												{
													if (text.Contains(targetParamName))
													{
														for (var index = 0; index < lines.Count; ++index)
														{
															var line = lines[index];
															if (line.Contains(targetParamName))
															{
																lines.Insert(index + 1, line.Replace(targetParamName, addingParamName));
																statement = SyntaxFactory.ParseStatement(string.Join("\r\n", lines));
																break;
															}
														}
													}
													else
													{
														if (lines.Count > 3 && lines[3].EndsWith(','))
														{
															lines.Insert(index + 1, lines[3]);
															statement = SyntaxFactory.ParseStatement(string.Join("\r\n", lines));
														}
													}
												}
											}
											break;

										case SyntaxKind.IfStatement:
											{
												var text = statement.ToFullString();
												if (text.Contains(targetParamName))
												{
													statements.Add(statement);
													text = text.Replace(targetParamName, addingParamName);
													if (!text.StartsWith("\r\n", StringComparison.Ordinal))
													{
														text = "\r\n" + text;
													}

													statement = SyntaxFactory.ParseStatement(text);
												}
											}
											break;
									}

									statements.Add(statement);
								}

								return method.WithBody(method.Body.WithStatements(SyntaxFactory.List(statements)));
							}
							#endregion // method RewriteBody
						}
						#endregion // method AddTypeParameter
					}
					#endregion // method VaryTypeParameters

					#region method VaryProcessParameters
					IEnumerable<MethodDeclarationSyntax> VaryProcessParameters(MethodDeclarationSyntax method)
					{
						yield return GenerateSubjectiveProcessParameters(method);
						yield return method;

						#region method GenerateSubjectiveProcessParameters
						MethodDeclarationSyntax GenerateSubjectiveProcessParameters(MethodDeclarationSyntax method)
						{
							var ret = method;
							foreach ((var parameter, var index) in method.ParameterList.Parameters.Where(_ => _.Identifier.Text.StartsWith("process", StringComparison.Ordinal)).WithIndices())
							{
								var process = ret.ParameterList.Parameters.First(_ => _.Identifier.Text == parameter.Identifier.Text);
								var typeName = process.Type.ToString().Replace(", PWalkContext", string.Empty) + " ";
								if (index > 0)
								{
									typeName = " " + typeName;
								}

								ret = ret.WithParameterList(ret.ParameterList.ReplaceNode(process, process.WithType(SyntaxFactory.ParseTypeName(typeName))));
							}

							ret = RewriteBody(ret);
							return ret;

							#region method RewriteBody
							MethodDeclarationSyntax RewriteBody(MethodDeclarationSyntax method)
							{
								var expression = $"TargetType{method.TypeParameterList}({string.Join(", ", method.ParameterList.Parameters.Select(_ => _.Identifier.Text))})";
								return RewriteExpressionBodyCore(method, ProcessParameterPattern.Replace(expression, "(target, context) => ${1}(target)"));
							}
							#endregion // method RewriteBody
						}
						#endregion // method GenerateSubjectiveProcessParameters
					}
					#endregion // method VaryProcessParameters
				}
				#endregion // method VaryTargetType
			}
			#endregion // CSharpSyntaxRewriter virtual member override

			#region private members
			private static readonly System.Text.RegularExpressions.Regex IndentPattern = new System.Text.RegularExpressions.Regex(@"\r\n(\s*)\S");
			private static readonly System.Text.RegularExpressions.Regex DocumentCommentElementLineFeedPattern = new System.Text.RegularExpressions.Regex(@"\r\n\s*///\s*");
			private static readonly System.Text.RegularExpressions.Regex ProcessParameterPattern = new System.Text.RegularExpressions.Regex(@"(process\d*)(?!es)");
			#endregion // private members
		}
		#endregion // class ManagerCodeGenerator

		#region class CodeGenerator
		private class CodeGenerator : CSharpSyntaxRewriter
		{
			#region CSharpSyntaxRewriter virtual member override
			public override SyntaxNode VisitClassDeclaration(ClassDeclarationSyntax node)
			{
				var ret = node;
				var members = new List<MemberDeclarationSyntax>();
				foreach (var member in node.Members)
				{
					var kind = member.Kind();
					switch (kind)
					{
						case SyntaxKind.MethodDeclaration:
							if (member.Modifiers.Any(_ => _.Kind() == SyntaxKind.PublicKeyword))
							{
								members.Add(member);
							}
							break;
					}
				}

				var retModifiers = ret.Modifiers;
				{
					var index = 0;
					while (index < retModifiers.Count)
					{
						if (retModifiers[index].Kind() > SyntaxKind.StaticKeyword)
						{
							break;
						}

						++index;
					}

					retModifiers = retModifiers.Insert(index, SyntaxFactory.Token(SyntaxKind.StaticKeyword));
				}

				return ret.Update(ret.AttributeLists, retModifiers, ret.Keyword,
					SyntaxFactory.Identifier(nameof(TestService)),
					ret.TypeParameterList, ret.BaseList, ret.ConstraintClauses,
					ret.OpenBraceToken.WithoutTrivia(), new SyntaxList<MemberDeclarationSyntax>(members), ret.CloseBraceToken.WithoutTrivia(),
					ret.SemicolonToken).WithoutTrivia();
			}
			#endregion // CSharpSyntaxRewriter virtual member override
		}
		#endregion // class CodeGenerator
		#endregion // private members
	}
}