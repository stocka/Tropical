Imports Microsoft.VisualStudio.TestTools.UnitTesting
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
      Return System.IO.Directory.GetCurrentDirectory()
    Else
      Return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), subDir)
    End If

  End Function

  ''' <summary>
  ''' Gets the full image path for the specified file.
  ''' </summary>
  ''' <param name="fileName">The name of the image file.</param>
  ''' <returns>The full image path for the specified file.</returns>
  Public Shared Function GetFullImagePath(fileName As String) As String
    Return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "TestImages", fileName)
  End Function

End Class
