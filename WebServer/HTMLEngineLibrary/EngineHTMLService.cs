using System.Reflection;
using System.Text.RegularExpressions;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;
using System.Net;

namespace HTMLEngineLibrary
{
    public class EngineHTMLService : IEngineHTMLService
    {
        private readonly Regex Regex = new Regex(@"{{ (.*?) }}");
        private readonly Regex RegexFor = new Regex(@"{{ (for .* in .*) }}");
        private readonly Regex RegexEndFor = new Regex(@"{{ endfor }}");
        private readonly Regex RegexIf = new Regex(@"{{ (if .*) }}");
        private readonly Regex RegexElseIf = new Regex(@"{{ (elseif .*) }}");
        private readonly Regex RegexElse = new Regex(@"{{ else }}");
        private readonly Regex RegexEndIf = new Regex(@"{{ endif }}");

        public string GetHTML(string template, object model)
        {
            if (model == null)
            {
                foreach (Match cmd in Regex.Matches(template))
                {
                    template = template.Replace(cmd.Value, "");
                }
                return template;
            }

            var delta = 0;
            var nesting = new Stack<Match>();
            var endings = new Queue<Match>();


            foreach (Match cmd in Regex.Matches(template))
            {
                if (RegexFor.IsMatch(cmd.Value))
                {
                    nesting.Push(cmd);
                }
                else if (RegexEndFor.IsMatch(cmd.Value))
                {
                    endings.Enqueue(cmd);
                }

                if (nesting.Count != 0
                    && nesting.Count == endings.Count)
                    (template, delta) = ProcessBlockFor(nesting, endings, template, model, delta);
            }
            delta = 0;
            foreach (Match cmd in Regex.Matches(template))
            {
                if (RegexIf.IsMatch(cmd.Value))
                {
                    nesting.Push(cmd);
                }
                else if (RegexEndIf.IsMatch(cmd.Value))
                {
                    endings.Enqueue(cmd);
                }

                if (nesting.Count != 0
                    && nesting.Count == endings.Count)
                    (template, delta) = ProcessBlockIf(nesting, endings, template, model, delta);
            }
            foreach (Match cmd in Regex.Matches(template))
            {
                ProcessProperty(ref template, cmd, model);
            }

            return template;
        }

        public string GetHTML(Stream pathTemplate, object model)
        {
            throw new NotImplementedException();
        }

        public string GetHTML(byte[] bytes, object model)
        {
            return GetHTML(Encoding.UTF8.GetString(bytes), model);
        }

        public byte[] GetHTMLInByte(string template, object model)
        {
            return Encoding.UTF8.GetBytes(GetHTML(template, model));
        }
        public byte[] GetHTMLInByte(Stream pathTemplate, object model)
        {
            throw new NotImplementedException();
        }

        public byte[] GetHTMLInByte(byte[] bytes, object model)
        {
            return Encoding.UTF8.GetBytes(GetHTML(Encoding.UTF8.GetString(bytes), model));
        }

        public Stream GetHTMLInStream(string template, object model)
        {
            throw new NotImplementedException();
        }

        public Stream GetHTMLInStream(Stream pathTemplate, object model)
        {
            throw new NotImplementedException();
        }

        public Stream GetHTMLInStream(byte[] bytes, object model)
        {
            throw new NotImplementedException();
        }

        public void GenerateAndSaveInDirectory(string template, string outputPath, string outputFileName, object model)
        {
            throw new NotImplementedException();
        }

        public void GenerateAndSaveInDirectory(Stream templatePath, string outputPath, string outputFileName, object model)
        {
            throw new NotImplementedException();
        }

        public void GenerateAndSaveInDirectory(byte[] bytes, string outputPath, string outputFileName, object model)
        {
            throw new NotImplementedException();
        }

        public Task GenerateAndSaveTask(string template, string outputPath, string outputFileName, object model)
        {
            throw new NotImplementedException();
        }

        public Task GenerateAndSaveTask(Stream templatePath, string outputPath, string outputFileName, object model)
        {
            throw new NotImplementedException();
        }

        public Task GenerateAndSaveTask(byte[] bytes, string outputPath, string outputFileName, object model)
        {
            throw new NotImplementedException();
        }

        private (string, int) ProcessBlockFor(Stack<Match> nesting, Queue<Match> endings, string template,
            object model, int delta)
        {
            string block;
            var headDelta = delta;

            for (int i = 0; i <= nesting.Count; i++)
            {
                var head = nesting.Pop();
                var end = endings.Dequeue();
                var length = end.Index + delta - head.Index - head.Length - headDelta;
                block = template.Substring(head.Index + head.Length + headDelta, length);

                foreach (Match cmd in Regex.Matches(block))
                    ProcessProperty(ref block, cmd, model);

                block = ProcessFor(block, head, model);

                delta += block.Length - (end.Index + end.Length - head.Index);
                template = template.Substring(0, head.Index + headDelta) + block
                    + template.Substring(head.Index + head.Length + headDelta + length + end.Length);
            }
            return (template, delta);
        }

        private (string, int) ProcessBlockIf(Stack<Match> nesting, Queue<Match> endings, string template,
            object model, int delta)
        {
            string block;
            var headDelta = delta;
            for (int i = 0; i <= nesting.Count; i++)
            {
                var head = nesting.Pop();
                var headValue = head.Groups[1].Value;
                var end = endings.Dequeue();
                var length = end.Index + delta - head.Index - head.Length - headDelta;
                block = template.Substring(head.Index + head.Length + headDelta, length);

                ProcessIf(ref block, headValue, model);

                delta += block.Length - (end.Index + end.Length - head.Index);
                template = template.Substring(0, head.Index + headDelta) + block
                    + template.Substring(head.Index + head.Length + headDelta + length + end.Length);
            }
            return (template, delta);
        }

        private (PropertyInfo, object) GetPropertyInfo(string template, string cmd, object model)
        {
            var parts = cmd.Split('.');

            if (parts.Length == 1)
            {
                return (model
                    .GetType()
                    .GetProperty(cmd),
                    model);
            }
            else
            {
                if (model.GetType().Name != parts[0])
                    return (null, model);

                PropertyInfo prop = model.GetType().GetProperty(parts[1]);
                foreach (var p in parts.Skip(2))
                {
                    model = prop.GetValue(model);
                    prop = model.GetType().GetProperty(p);
                }
                return (prop, model);
            }
        }

        private bool GetCondition(string condition, object model)
        {
            ScriptState state;

            state = CSharpScript.RunAsync(condition).Result;

            return (bool)state.ReturnValue;
        }

        private bool CheckVariable(string condition, object model)
        {
            var parts = condition.Split(' ');
            if ((parts[0].All(c => char.IsDigit(c)) || parts[0][0] == '"' || parts[0] == "null")
                && (parts[2].All(c => char.IsDigit(c)) || parts[2][0] == '"' || parts[2] == "null"))
                return true;
            return false;
        }

        private void ProcessIf(ref string block, string headValue, object model)
        {
            var elseIf = RegexElseIf.Match(block);
            var elseM = RegexElse.Match(block);

            foreach (var part in headValue.Split(' ').Skip(1))
                ProcessProperty(ref headValue, part, model);

            var conditionIf = string.Join(' ', headValue.Split(' ').Skip(1));
            var conditionElseIf = string.Join(' ', elseIf.Groups[1].Value.Split(' ').Skip(1));
            if (!CheckVariable(conditionIf, model))
            {
                block = $"{{{{ {headValue} }}}}" + block + "{{ endif }}";
                return;
            }

            if (GetCondition(conditionIf, model))
            {
                if (elseIf.Value != "")
                    block = block[..elseIf.Index];
                else if (elseM.Value != "")
                    block = block[..elseM.Index];
                else
                    block = block[..block.Length];
            }
            else if (elseIf.Value != ""
                && GetCondition(conditionElseIf, model))
            {
                if (elseM.Value != "")
                    block = block.Substring(elseIf.Index + elseIf.Length,
                        elseM.Index - elseIf.Index - elseIf.Length);
                else
                    block = block.Substring(elseIf.Index + elseIf.Length,
                        block.Length - elseIf.Index - elseIf.Length);
            }
            else
            {
                if (elseM.Value != "")
                    block = block.Substring(elseM.Index + elseM.Length,
                        block.Length - elseM.Index - elseM.Length);
                else if (elseIf.Value != "")
                    block = block.Substring(elseIf.Index + elseIf.Length,
                        block.Length - elseIf.Index - elseIf.Length);
                else
                    block = "";
            }

            block = block.Trim();
        }

        private string ProcessFor(string block, Match match, object model)
        {
            var builder = new StringBuilder();
            var parts = match.Groups[1].Value.Split(' ');
            var (prop, obj) = GetPropertyInfo(block, parts[3], model);
            var lst = (IEnumerable<object>)prop.GetValue(obj);
            foreach (var item in lst)
            {
                builder.Append(block.Replace($"{{{{ {parts[1]} }}}}", item.ToString()).Replace(parts[1], item.GetType() == typeof(string) ? $"\"{item}\"" : item.ToString()));
                Console.WriteLine(builder.ToString());
            }
            block = builder.ToString();

            var delta = 0;
            var nesting = new Stack<Match>();
            var endings = new Queue<Match>();
            foreach (Match cmd in Regex.Matches(block))
            {
                if (RegexIf.IsMatch(cmd.Value))
                {
                    nesting.Push(cmd);
                }
                else if (RegexEndIf.IsMatch(cmd.Value))
                {
                    endings.Enqueue(cmd);
                }

                if (nesting.Count == endings.Count)
                    (block, delta) = ProcessBlockIf(nesting, endings, block, model, delta);
            }

            return block;
        }

        private void ProcessProperty(ref string block, Match cmd, object model)
        {
            if (model.GetType() == typeof(Tuple)) {
                var t = (Tuple<string, object>)model;
                block = block.Replace(cmd.Value, t.Item2.ToString());
            }

            var (prop, obj) = GetPropertyInfo(block, cmd.Groups[1].Value, model);
            if (prop != null)
                block = block.Replace(cmd.Value, prop.GetValue(model).ToString());
        }

        private void ProcessProperty(ref string block, string cmd, object model)
        {
            if (model.GetType().Name == "Tuple`2")
            {
                var t = (Tuple<string, Cookie>)model;
                block = block.Replace(t.Item1, t.Item2 != null ? $"\"{t.Item2.Value}\"" : "null");
            }

            var (prop, obj) = GetPropertyInfo(block, cmd, model);
            if (prop != null)
                block = block.Replace(cmd, prop.GetValue(model).ToString());
        }
    }
}
