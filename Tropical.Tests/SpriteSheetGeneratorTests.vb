Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteSheetGeneratorTests

  ''' <summary>
  ''' The log used for any <see cref="Controllers.SpriteSheetGenerator">sprite sheet generators</see>
  ''' instantiated by this class.
  ''' </summary>
  Private Log As TestLogger

  ''' <summary>
  ''' Gets or sets the test context.
  ''' </summary>
  ''' <value>
  ''' The test context.
  ''' </value>
  Public Property TestContext() As TestContext

  ''' <summary>
  ''' Gets the destination path for the sprite sheet
  ''' in this test run. Dependent on the
  ''' <see cref="TestContext" /> being properly initialized.
  ''' </summary>
  ''' <value>
  ''' The destination path.
  ''' </value>
  Public ReadOnly Property DestinationPath() As String
    Get
      Return GetDestinationPath(subDir:=TestContext.TestName)
    End Get
  End Property

  <TestInitialize()>
  Public Sub Initialize()
    Me.Log = New TestLogger()
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the generation of a sprite sheet when no sprites have been added.")>
  Public Sub TestEmpty()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    Dim generator As Controllers.SpriteSheetGenerator = GetGenerator(sheet)

    ' Ensure calculations are correct
    AssertSpriteCalculations(generator, 0, 0, 0)

    ' Make sure generation doesn't succeed
    Assert.IsFalse(generator.Generate(Me.DestinationPath))

    ' Make sure an error log entry was created.
    Assert.AreEqual(1, Log.ErrorEntries.Count)
    Assert.AreEqual("No sprites have been added to the sheet.", Log.ErrorEntries.First().Message)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of a single sprite, with no hover image specified.")>
  Public Sub TestOne_NoHover()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add a sprite, no hover
    AddSprite(sheet, "sprite", "icon_16.png", Nothing)

    Dim generator As Controllers.SpriteSheetGenerator = GetGenerator(sheet)

    ' Ensure calculations are correct for our single image
    AssertSpriteCalculations(generator, 1, 16, 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(Me.DestinationPath))

    ' Make sure no errors/warnings were logged
    AssertNoWarningsOrErrors()

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of a single sprite, with only a hover image specified.")>
  Public Sub TestOne_OnlyHover()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add a sprite, only hover
    AddSprite(sheet, "sprite", Nothing, "icon_hover_16.png")

    Dim generator As Controllers.SpriteSheetGenerator = GetGenerator(sheet)

    ' Ensure calculations are correct for our single image
    AssertSpriteCalculations(generator, 1, 16, 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(Me.DestinationPath))

    ' Make sure no errors/warnings were logged
    AssertNoWarningsOrErrors()

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of a single sprite, with a hover image specified.")>
  Public Sub TestOne_WithHover()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add a sprite, include hover
    AddSprite(sheet, "sprite", "icon_16.png", "icon_hover_16.png")

    Dim generator As Controllers.SpriteSheetGenerator = GetGenerator(sheet)

    ' Ensure calculations are correct for our two sprites (one for the
    ' regular image, and the other for the hover)
    AssertSpriteCalculations(generator, 2, 16, 32)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(Me.DestinationPath))

    ' Make sure no errors/warnings were logged
    AssertNoWarningsOrErrors()

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of two sprites, both with a hover image specified.")>
  Public Sub TestTwo_WithHover()

    Dim destPath As String = GetDestinationPath(Me.DestinationPath)

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add two sprites, include hover
    AddSprite(sheet, "radioactive", "icon_16.png", "icon_hover_16.png")
    AddSprite(sheet, "information", "alticon_16.png", "alticon_hover_16.png")

    Dim generator As Controllers.SpriteSheetGenerator = GetGenerator(sheet)

    ' This should actually create four images now,
    ' as each will have hover.
    AssertSpriteCalculations(generator, 4, 16, 64)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    AssertNoWarningsOrErrors()

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of all different formats of sprite images.")>
  Public Sub TestAllImageFormats()

    Dim destPath As String = GetDestinationPath(Me.DestinationPath)

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add two sprites, include hover
    AddSprite(sheet, "radioactive", "icon_16.png", "icon_hover_16.png")
    AddSprite(sheet, "information", "alticon_16.png", "alticon_hover_16.png")

    ' Now we add all of the other images that we have, save for
    ' alternatively-sized ones
    AddSprite(sheet, "gif", "icon_16.gif", Nothing)
    AddSprite(sheet, "gif-notransparency", "icon_notrans_16.gif", Nothing)
    AddSprite(sheet, "jpg", "icon_16.jpg", Nothing)
    AddSprite(sheet, "jpg-grayscale", "icon_grayscale_16.jpg", Nothing)
    AddSprite(sheet, "png-notransparency", "icon_notrans_16.png", Nothing)
    AddSprite(sheet, "png-indexed", "icon_indexed_16.png", Nothing)
    AddSprite(sheet, "png-indexed-notransparency", "icon_indexed_notrans_16.png", Nothing)

    Dim generator As Controllers.SpriteSheetGenerator = GetGenerator(sheet)

    ' Get our sprite count, adding 2 for the hover images we also specified above.
    Dim spriteCount As Int32 = sheet.Sprites.Count + 2

    ' This should actually create four images now,
    ' as each will have hover.
    AssertSpriteCalculations(generator, spriteCount, 16, spriteCount * 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    AssertNoWarningsOrErrors()

  End Sub

#Region "Helper Methods/Properties"

  ''' <summary>
  ''' Asserts that the provided generator instance has correctly-calculated
  ''' the number of sprites and the dimensions of the sprite sheet.
  ''' </summary>
  ''' <param name="generator">The sprite sheet generator.</param>
  ''' <param name="expectedSpriteCount">The expected sprite count.</param>
  ''' <param name="expectedSheetHeight">The expected height of the sheet.</param>
  ''' <param name="expectedSheetWidth">The expected width of the sheet.</param>
  Private Sub AssertSpriteCalculations(generator As Controllers.SpriteSheetGenerator,
                                       expectedSpriteCount As Int32,
                                       expectedSheetHeight As Int32,
                                       expectedSheetWidth As Int32)

    Assert.AreEqual(expectedSpriteCount, generator.SpriteCount)
    Assert.AreEqual(expectedSheetHeight, generator.Dimensions.Height)
    Assert.AreEqual(expectedSheetWidth, generator.Dimensions.Width)

  End Sub

  ''' <summary>
  ''' Asserts that no warnings or errors have been recorded in the
  ''' <see cref="Log">log</see>.
  ''' </summary>
  Private Sub AssertNoWarningsOrErrors()

    Assert.AreEqual(0, Log.ErrorEntries.Count)
    Assert.AreEqual(0, Log.WarningEntries.Count)

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

#End Region

End Class
