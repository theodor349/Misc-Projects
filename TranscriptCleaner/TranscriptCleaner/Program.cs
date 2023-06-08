using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

string inputFilePath = "input.txt";
string outputFilePath = "output.txt";
var headerRegx = new Regex(@"\d\d:\d\d:\d\d .*");
var timeRegx = new Regex(@"\d\d:\d\d:\d\d");
var speakerRegx = new Regex(@"[A-z]+.*");

var lines = File.ReadAllLines(inputFilePath, Encoding.GetEncoding("iso-8859-1"));
var transcripts = ParseLines(lines);
var result = WriteTransripts(transcripts);
File.WriteAllLines(outputFilePath, result, Encoding.GetEncoding("iso-8859-1"));

List<string> WriteTransripts(List<Transcript> transcripts)
{
	var lines = new List<string>();
	foreach (var t in transcripts)
	{
		lines.Add("");
        lines.Add($"{t.Time.ToString("HH:mm:ss")} {t.Speaker}");
		var content = "";
		foreach (var l in t.Content)
		{
			content += $" {l.Trim()}";
        }
		content = content.Trim();
		lines.Add(content);
	}
	return lines;
}

List<Transcript> ParseLines(string[] lines)
{
	var transcripts = new List<Transcript>();
	Transcript transcript = new Transcript(TimeOnly.MinValue, "--Remove--", new List<string>());
	foreach (var line in lines)
	{
		if(headerRegx.IsMatch(line))
		{
			var timeMatch = timeRegx.Match(line).Value;
			var speakerMatch = speakerRegx.Match(line).Value;

            var time = TimeOnly.ParseExact(timeMatch, "HH:mm:ss", CultureInfo.InvariantCulture);
			var speaker = speakerMatch;
			if (!transcript.Speaker.Equals(speaker) || transcript.Time.Minute != time.Minute)
			{
				transcripts.Add(transcript);
                transcript = new Transcript(time, speaker, new List<string>());
            }
        }
		else if (string.IsNullOrWhiteSpace(line))
		{
			continue;
        }
		else
		{
			transcript.Content.Add(line);
		}
	}
	transcripts.Add(transcript);
	return transcripts.Where(t => !t.Speaker.Equals("--Remove--")).ToList();
}



record Transcript(TimeOnly Time, string Speaker, List<string> Content);