Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteSheetGeneratorTests

  Private Log As TestLogger

  <TestInitialize()>
  Public Sub Initialize()
    Me.Log = New TestLogger()
  End Sub

  ''' <summary>
  ''' Gets a <see cref="Controllers.SpriteSheetGenerator" /> instance
  ''' for the specified sprite sheet, initialized with properties
  ''' such as this instance's logger.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  ''' <returns>The initialized generator.</returns>
  Private Function GetGenerator(sheet As Models.SpriteSheet) As Controllers.SpriteSheetGenerator

    Dim generator As New Controllers.SpriteSheetGenerator(sheet)

    ' Set the logger
    generator.Logger = Log

    Return generator

  End Function

  ''' <summary>
  ''' Gets the destination path for the sprite sheet.
  ''' </summary>
  ''' <param name="subDir">The subdirectory to use. Optional.</param>
  ''' <returns>The destination path for the sprite sheet.</returns>
  Private Function GetDestinationPath(Optional subDir As String = Nothing)

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
  Private Function GetFullImagePath(fileName As String) As String
    Return System.IO.Path.Combine(System.IO.Directory.GetCurrentDirectory(), "TestImages", fileName)
  End Function

  ''' <summary>
  ''' Adds a sprite to the stylesheet.
  ''' </summary>
  ''' <param name="sheet">The sheet.</param>
  ''' <param name="className">The CSS class of the sprite.</param>
  ''' <param name="imageName">The file name of the standard image.
  ''' Path information should be omitted.</param>
  ''' <param name="hoverImageName">The file name of the hover image.
  ''' Path information should be omitted.</param>
  Private Sub AddSprite(sheet As Models.SpriteSheet, className As String, imageName As String, hoverImageName As String)

    Dim sprite As New Models.Sprite()
    With sprite

      .ClassName = className

      If Not String.IsNullOrWhiteSpace(imageName) Then
        .ImagePath = GetFullImagePath(imageName)
      End If

      If Not String.IsNullOrWhiteSpace(hoverImageName) Then
        .HoverImagePath = GetFullImagePath(hoverImageName)
      End If

    End With

    sheet.Sprites.Add(sprite)

  End Sub

  <TestMethod()>
  Public Sub TestEmpty()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    Dim generator = GetGenerator(sheet)

    Assert.AreEqual(0, generator.SpriteCount)
    Assert.AreEqual(0, generator.Dimensions.Height)
    Assert.AreEqual(0, generator.Dimensions.Width)

    ' Make sure generation doesn't succeed
    Assert.IsFalse(generator.Generate(GetDestinationPath()))

    ' Make sure an error log entry was created.
    Assert.AreEqual(1, Log.ErrorEntries.Count)
    Assert.AreEqual("No sprites have been added to the sheet.", Log.ErrorEntries.First().Message)

  End Sub

  <TestMethod()>
  Public Sub TestOne_NoHover()

    Dim destPath As String = GetDestinationPath("TestOne_NoHover")

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add a sprite, no hover
    AddSprite(sheet, "sprite", "icon_16.png", Nothing)
    
    Dim generator = GetGenerator(sheet)

    Assert.AreEqual(1, generator.SpriteCount)
    Assert.AreEqual(16, generator.Dimensions.Height)
    Assert.AreEqual(16, generator.Dimensions.Width)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    Assert.AreEqual(0, Log.ErrorEntries.Count)
    Assert.AreEqual(0, Log.WarningEntries.Count)

  End Sub

  <TestMethod()>
  Public Sub TestOne_WithHover()

    Dim destPath As String = GetDestinationPath("TestOne_WithHover")

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add a sprite, include hover
    AddSprite(sheet, "sprite", "icon_16.png", "icon_hover_16.png")

    Dim generator = GetGenerator(sheet)

    ' This should actually create two images now, one for the regular
    ' and the other for the hover.
    Assert.AreEqual(2, generator.SpriteCount)
    Assert.AreEqual(16, generator.Dimensions.Height)
    Assert.AreEqual(32, generator.Dimensions.Width)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    Assert.AreEqual(0, Log.ErrorEntries.Count)
    Assert.AreEqual(0, Log.WarningEntries.Count)

  End Sub

End Class
