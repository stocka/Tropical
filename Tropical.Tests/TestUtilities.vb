Imports Microsoft.VisualStudio.TestTools.UnitTesting
Imports System.IO
Imports Tropical.Models

''' <summary>
''' Contains utility methods to be shared across test classes.
''' </summary>
Public Class TestUtilities

  ''' <summary>
  ''' Prevents a default instance of the <see cref="TestUtilities"/> class from being created.
  ''' </summary>
  Private Sub New()
  End Sub

  ''' <summary>
  ''' Asserts that no warnings or errors have been recorded in the
  ''' provided <see cref="Log">log</see>.
  ''' </summary>
  Public Shared Sub AssertNoWarningsOrErrors(logger As TestLogger)

    Assert.AreEqual(0, logger.ErrorEntries.Count)
    Assert.AreEqual(0, logger.WarningEntries.Count)

  End Sub

  ''' <summary>
  ''' Gets a <see cref="Controllers.SpriteSheetGenerator" /> instance
  ''' for the specified sprite sheet, initialized with properties
  ''' such as the logger.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  ''' <param name="logger">The logger to use.</param>
  ''' <returns>The initialized generator.</returns>
  Public Shared Function GetGenerator(sheet As Models.SpriteSheet,
                                      logger As ILogger) As Controllers.SpriteSheetGenerator

    Dim generator As New Controllers.SpriteSheetGenerator(sheet)

    ' Set the logger
    generator.Logger = logger

    Return generator

  End Function

  ''' <summary>
  ''' Gets a standard set of options.
  ''' </summary>
  ''' <returns>An initialized set of options.</returns>
  Public Shared Function GetStandardOptions() As Models.SpriteSheetContentGeneratorOptions

    Dim options As New Models.SpriteSheetContentGeneratorOptions()

    With options
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageWidth = 16
      .ImageHeight = 16
      .FilterClassNames = {"accent"}
      .HoverClassNames = {"hover"}
      .ClassDelimiters = {" "c, "-"c, "_"c}
      .FileExtensions = {"jpeg", "jpg", "png", "gif"}
    End With

    Return options

  End Function

  ''' <summary>
  ''' Adds a sprite to the stylesheet.
  ''' </summary>
  ''' <param name="sheet">The sheet.</param>
  ''' <param name="className">The CSS class of the sprite.</param>
  ''' <param name="filterClassName">The filtering CSS class to use
  ''' for the sprite.</param>
  ''' <param name="imageName">The file name of the standard image.
  ''' Path information should be omitted.</param>
  ''' <param name="hoverImageName">The file name of the hover image.
  ''' Path information should be omitted.</param>
  Public Shared Sub AddSprite(sheet As Models.SpriteSheet,
                              className As String,
                              filterClassName As String,
                              imageName As String,
                              hoverImageName As String)

    Dim sprite As New Models.Sprite()
    With sprite

      .ClassName = className

      If Not String.IsNullOrWhiteSpace(imageName) Then
        .ImagePath = GetFullImagePath(imageName)
      End If

      If Not String.IsNullOrWhiteSpace(hoverImageName) Then
        .HoverImagePath = GetFullImagePath(hoverImageName)
      End If

      If Not String.IsNullOrWhiteSpace(filterClassName) Then
        .FilterClassName = filterClassName
      End If

    End With

    sheet.Sprites.Add(sprite)

  End Sub

  ''' <summary>
  ''' Gets the destination path for the sprite sheet.
  ''' </summary>
  ''' <param name="subDir">The subdirectory to use. Optional.</param>
  ''' <returns>The destination path for the sprite sheet.</returns>
  Public Shared Function GetDestinationPath(Optional subDir As String = Nothing)

    If String.IsNullOrWhiteSpace(subDir) Then
      Return Directory.GetCurrentDirectory()
    Else
      Return Path.Combine(Directory.GetCurrentDirectory(), subDir)
    End If

  End Function

  ''' <summary>
  ''' Gets the path to the test images directory.
  ''' </summary>
  ''' <returns>The path to the test images directory.</returns>
  Public Shared Function GetTestImagesDirectory() As String
    Return Path.Combine(Directory.GetCurrentDirectory(), "TestImages")
  End Function

  ''' <summary>
  ''' Gets the full image path for the specified file.
  ''' </summary>
  ''' <param name="fileName">The name of the image file.</param>
  ''' <returns>The full image path for the specified file.</returns>
  Public Shared Function GetFullImagePath(fileName As String) As String
    Return Path.Combine(Directory.GetCurrentDirectory(), "TestImages", fileName)
  End Function

  ''' <summary>
  ''' Asserts that two directory paths are equal. This will perform normalization
  ''' for trailing directory separators and use a case-insensitive comparison.
  ''' </summary>
  ''' <param name="expectedDirectory">The expected directory path.</param>
  ''' <param name="actualDirectory">The actual directory path.</param>
  Public Shared Sub AssertDirectoriesEqual(expectedDirectory As String,
                                           actualDirectory As String)

    ' Normalize trailing slashes.
    ' We call this twice, once to handle if the expected directory
    ' ends with the regular separator and once again to handle if it ends
    ' with the alternate separator.
    actualDirectory = NormalizeTrailingDirSeparator(expectedDirectory, actualDirectory, Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar)
    actualDirectory = NormalizeTrailingDirSeparator(expectedDirectory, actualDirectory, Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar)

    ' Compare, but ignore case as these are paths.
    Assert.AreEqual(expectedDirectory, actualDirectory, True)

  End Sub

  ''' <summary>
  ''' Normalizes the trailing directory separator on the <paramref name="actualDirectory" />
  ''' to match that of the <paramref name="expectedDirectory" />.
  ''' </summary>
  ''' <param name="expectedDirectory">The expected directory.</param>
  ''' <param name="actualDirectory">The actual directory.</param>
  ''' <param name="separator">The separator to search for.</param>
  ''' <param name="altSeparator">The alternate separator. If this is present on the
  ''' actual directory, but not on the expected directory, it will be removed
  ''' and replaced with the standard separator.</param>
  ''' <returns>The normalized value of <paramref name="actualDirectory" />.</returns>
  Private Shared Function NormalizeTrailingDirSeparator(expectedDirectory As String,
                                                        actualDirectory As String,
                                                        separator As Char,
                                                        altSeparator As Char) As String

    ' See if they both end with the separator.
    If expectedDirectory.EndsWith(separator) AndAlso
      Not actualDirectory.EndsWith(separator) Then

      ' Trim out the alt-separator if it's there
      If actualDirectory.EndsWith(altSeparator) Then
        actualDirectory = actualDirectory.TrimEnd(altSeparator)
      End If

      actualDirectory = actualDirectory & separator

    ElseIf Not expectedDirectory.EndsWith(separator) AndAlso
      actualDirectory.EndsWith(separator) Then

      ' Trim out the separator if it's present on the actual result
      ' but not on the expected result.
      actualDirectory = actualDirectory.TrimEnd(separator)

    End If

    Return actualDirectory

  End Function

End Class
