namespace FlowinglyTest;

public static class Extensions
{
    public static async Task<string> ReadAsStringAsync(this IFormFile file)
    {
        var result = new System.Text.StringBuilder();
        using (var reader = new StreamReader(file.OpenReadStream()))
        {
            while (reader.Peek() >= 0)
                result.AppendLine(await reader.ReadLineAsync());
        }
        return result.ToString();
    }
}
