Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' Contains unit tests for the <see cref="Controllers.SpriteSheetGenerator" />.
''' </summary>
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
      Return TestUtilities.GetDestinationPath(subDir:=TestContext.TestName)
    End Get
  End Property

  <TestInitialize()>
  Public Sub Initialize()
    Me.Log = New TestLogger()
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests that the Logger property cannot be set to null.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub TestNullLogger()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' This should throw an ArgumentNullException
    generator.Logger = Nothing

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

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

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
    TestUtilities.AddSprite(sheet, "sprite", Nothing, "icon_16.png", Nothing)

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' Ensure calculations are correct for our single image
    AssertSpriteCalculations(generator, 1, 16, 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(Me.DestinationPath))

    ' Make sure no errors/warnings were logged
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

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
    TestUtilities.AddSprite(sheet, "sprite", Nothing, Nothing, "icon_hover_16.png")

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' Ensure calculations are correct for our single image
    AssertSpriteCalculations(generator, 1, 16, 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(Me.DestinationPath))

    ' Make sure no errors/warnings were logged
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

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
    TestUtilities.AddSprite(sheet, "sprite", Nothing, "icon_16.png", "icon_hover_16.png")

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' Ensure calculations are correct for our two sprites (one for the
    ' regular image, and the other for the hover)
    AssertSpriteCalculations(generator, 2, 16, 32)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(Me.DestinationPath))

    ' Make sure no errors/warnings were logged
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of a single sprite, with a hover image (identical to the standard image) specified.")>
  Public Sub TestOne_SameImage()

    Dim destPath As String = TestUtilities.GetDestinationPath(Me.DestinationPath)

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add one sprite, but make the hover image same as the regular one.
    ' To make it tricky, let's capitalize that second name
    TestUtilities.AddSprite(sheet, "radioactive", Nothing, "icon_16.png", "ICON_16.PNG")

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' This should only create one sprite, as the standard and hover images
    ' are identical
    AssertSpriteCalculations(generator, 1, 16, 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of two sprites, both with a hover image specified and one acting as a filtered accent.")>
  Public Sub TestTwo_Accent()

    Dim destPath As String = TestUtilities.GetDestinationPath(Me.DestinationPath)

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add two sprites, include hover
    TestUtilities.AddSprite(sheet, "radioactive", Nothing, "icon_16.png", "icon_hover_16.png")
    TestUtilities.AddSprite(sheet, "radioactive", "accent", "icon_accent_16.png", "icon_accent_hover_16.png")

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' This should actually create four images now,
    ' as each will have hover.
    AssertSpriteCalculations(generator, 4, 16, 64)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Generation")>
  <Description("Tests the addition of all different formats of sprite images.")>
  Public Sub TestAllImageFormats()

    Dim destPath As String = TestUtilities.GetDestinationPath(Me.DestinationPath)

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ' Add two sprites, include hover
    TestUtilities.AddSprite(sheet, "radioactive", Nothing, "icon_16.png", "icon_hover_16.png")
    TestUtilities.AddSprite(sheet, "information", Nothing, "alticon_16.png", "alticon_hover_16.png")

    ' Now we add all of the other images that we have, save for
    ' alternatively-sized ones
    TestUtilities.AddSprite(sheet, "gif", Nothing, "icon_16.gif", Nothing)
    TestUtilities.AddSprite(sheet, "gif-notransparency", Nothing, "icon_notrans_16.gif", Nothing)
    TestUtilities.AddSprite(sheet, "jpg", Nothing, "icon_16.jpg", Nothing)
    TestUtilities.AddSprite(sheet, "jpg-grayscale", Nothing, "icon_grayscale_16.jpg", Nothing)
    TestUtilities.AddSprite(sheet, "png-notransparency", Nothing, "icon_notrans_16.png", Nothing)
    TestUtilities.AddSprite(sheet, "png-indexed", Nothing, "icon_indexed_16.png", Nothing)
    TestUtilities.AddSprite(sheet, "png-indexed-notransparency", Nothing, "icon_indexed_notrans_16.png", Nothing)

    Dim generator As Controllers.SpriteSheetGenerator = TestUtilities.GetGenerator(sheet, Me.Log)

    ' Get our sprite count, adding 2 for the hover images we also specified above.
    Dim spriteCount As Int32 = sheet.Sprites.Count + 2

    ' This should actually create four images now,
    ' as each will have hover.
    AssertSpriteCalculations(generator, spriteCount, 16, spriteCount * 16)

    ' Make sure generation succeeds
    Assert.IsTrue(generator.Generate(destPath))

    ' Make sure no errors/warnings were logged
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

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
  Private Shared Sub AssertSpriteCalculations(generator As Controllers.SpriteSheetGenerator,
                                              expectedSpriteCount As Int32,
                                              expectedSheetHeight As Int32,
                                              expectedSheetWidth As Int32)

    Assert.AreEqual(expectedSpriteCount, generator.SpriteCount)
    Assert.AreEqual(expectedSheetHeight, generator.Dimensions.Height)
    Assert.AreEqual(expectedSheetWidth, generator.Dimensions.Width)

  End Sub

#End Region

End Class
