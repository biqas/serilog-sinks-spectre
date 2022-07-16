using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Serilog.Events;
using Serilog.Parsing;
using Serilog.Sinks.Spectre.Extensions;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace Serilog.Sinks.Spectre.Renderers
{
	public class MessageTemplateOutputTokenRenderer : ITemplateTokenRenderer
	{
		readonly PropertyToken token;

		public MessageTemplateOutputTokenRenderer(PropertyToken token)
		{
			this.token = token;
		}

		public IEnumerable<IRenderable> Render(LogEvent logEvent)
		{
			foreach (var token in logEvent.MessageTemplate.Tokens.OfType<PropertyToken>())
			{
				if (logEvent.Properties.ContainsKey(token.PropertyName))
				{
					var value = logEvent.Properties[token.PropertyName]
							.ToString(token.Format, default)
							.Exec(Markup.Escape)
							.Exec(Style.DefaultStyle.HighlightProp);

					value = value.Replace("\"", "");

					var propValue = new LogEventProperty(
						token.PropertyName, new ScalarValue(value));
					
					logEvent.AddOrUpdateProperty(propValue);
				}
			}
			
			yield return new Markup(logEvent.MessageTemplate.Render(logEvent.Properties));
		}
	}
}