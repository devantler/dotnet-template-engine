using System.Text;

namespace Devantler.TemplateEngine;

/// <inheritdoc />
public class Generator(ITemplateEngine templateEngine) : IGenerator
{
  readonly ITemplateEngine _templateEngine = templateEngine;

  /// <inheritdoc />
  public Task<string> GenerateAsync(string templateContentOrPath, object model) =>
   _templateEngine.RenderAsync(templateContentOrPath, model);

  /// <inheritdoc />
  public async Task GenerateAsync(
    string outputPath,
    string templateContentOrPath,
    object model,
    FileMode fileMode = FileMode.CreateNew
  )
  {
    string? directoryName = Path.GetDirectoryName(outputPath);
    if (string.IsNullOrEmpty(directoryName))
      throw new ArgumentException("The output path is invalid.", nameof(outputPath));
    if (!Directory.Exists(directoryName))
      _ = Directory.CreateDirectory(directoryName);

    var fileStream = new FileStream(outputPath, fileMode, FileAccess.Write);
    string renderedTemplate = await _templateEngine.RenderAsync(templateContentOrPath, model);
    await fileStream.WriteAsync(Encoding.UTF8.GetBytes(renderedTemplate));
    await fileStream.FlushAsync();
    fileStream.Close();
  }
}