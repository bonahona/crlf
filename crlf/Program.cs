using CommandLine;

class Program
{
    public class CommandOptions
    {
        [Option('f', "folder", Required = true, HelpText = "Root folder to search through", Default = ".")]
        public string RootFolder { get; set; } = ".";

        [Option('e', "ending", Required = true, HelpText = "File ending to replace")]
        public string Ending { get; set; } = string.Empty;
    }

    public static void Main(string[] args) {
        new Program(args);
    }

    private Program(string[] args)
    {
        Parser.Default.ParseArguments<CommandOptions>(args)
            .WithParsed<CommandOptions>(o => {
                Console.WriteLine($"Replacing \\R\\N with \\R in folder {o.RootFolder} for files ending with {o.Ending}");
                RunFileEndingConversion(o.RootFolder, o.Ending);
                Console.WriteLine("Done");
            });
    }

    private void RunFileEndingConversion(string root, string fileEnding)
    {
        foreach(var file in GetAllFilesFromRoot(root, fileEnding)) {
            ReplaceFileEndings(file);
        }
    }

    private List<string> GetAllFilesFromRoot(string root, string fileEnding)
    {
        var result = new List<string>();
        GetAllFoldersRecursive(root, fileEnding, result);
        return result;
    }

    private void GetAllFoldersRecursive(string rootFolder, string fileEnding, List<string> result) {
        foreach (var directory in Directory.GetDirectories(rootFolder)) {
            GetAllFoldersRecursive(directory, fileEnding, result);
        }

        foreach (var file in Directory.GetFiles(rootFolder)) {
            var fileInfo = new FileInfo(file);
            if(fileInfo.Extension.ToLower() == fileEnding) {
                result.Add(file);
            }
        }
    }

    private void ReplaceFileEndings(string file)
    {
        Console.WriteLine($"Processing {file}");
        var fileContent = File.ReadAllText(file);
        fileContent.Replace("\r\n", "\n");
        File.WriteAllText(file, fileContent);
    }
}