namespace Lyt.Tools;

public sealed class FileManager
{
    public static readonly FileManager Instance = new FileManager();
    public const string Assets = @"Assets/Dictionaries"  ;

    public string LoadTextResource(string folder, string name)
    {
        var uriSource = new Uri(string.Format("pack://application:,,,/{0}/{1}/{2}.txt", Assets , folder, name ));
        try
        {
            var streamResourceInfo = Application.GetResourceStream(uriSource);
            using (var reader = new StreamReader(streamResourceInfo.Stream))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
            if (Debugger.IsAttached) { Debugger.Break(); }
            return null;
        }
    }


    public async Task<bool> Exists(string fileName)
    {
        try
        {
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return true;
    }

    public async Task<string> ReadTextFile(string fileName)
    {
        try
        {
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }

        return string.Empty;
    }

    public async Task WriteTextFile(string fileName, string contents)
    {
        try
        {
        }
        catch (Exception e)
        {
            Debug.WriteLine(e);
        }
    }
}
