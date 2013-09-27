Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteSheetContentGeneratorTests

  ''' <summary>
  ''' The log used for any content generation testing.
  ''' </summary>
  Private Log As TestLogger

  ''' <summary>
  ''' Gets or sets the test context.
  ''' </summary>
  ''' <value>
  ''' The test context.
  ''' </value>
  Public Property TestContext() As TestContext

  <TestInitialize()>
  Public Sub Initialize()
    Me.Log = New TestLogger()
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator cannot be initialized with null options.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub TestNullOptions()

    Dim contentGenerator As New Controllers.SpriteSheetContentGenerator(Nothing)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator cannot be initialized with invalid options.")>
  <ExpectedException(GetType(System.ComponentModel.DataAnnotations.ValidationException))>
  Public Sub TestInvalidOptions()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    options.ImageWidth = 0

    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the Logger property cannot be set to null.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub TestNullLogger()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    contentGenerator.Logger = Nothing

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator does not accept a null/whitespace source directory.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub TestNullDirectory()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    contentGenerator.Generate("   ")

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator does not accept an invalid source directory.")>
  <ExpectedException(GetType(ArgumentException), AllowDerivedTypes:=False)>
  Public Sub TestInvalidDirectory()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    contentGenerator.Generate("|||$CLOCK")

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator does not accept a nonexistent source directory.")>
  <ExpectedException(GetType(IO.DirectoryNotFoundException))>
  Public Sub TestNonExistentDirectory()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    contentGenerator.Generate(Guid.NewGuid.ToString())

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator does not accept a source directory that is a file.")>
  <ExpectedException(GetType(IO.DirectoryNotFoundException))>
  Public Sub TestFileDirectory()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()
    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    contentGenerator.Generate(TestUtilities.GetFullImagePath("image_16.png"))

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator behaves normally. This will also test the file search pattern.")>
  Public Sub TestContentGeneration()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()

    ' Let's avoid confusing the issue by only using our 16x16 PNGs.
    options.FileSearchPattern = "*_16.png"

    ' Set up accent/hover classes
    options.FilterClassNames = {"accent"}
    options.HoverClassNames = {"hover"}

    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    Dim sheet As Models.SpriteSheet = contentGenerator.Generate(TestUtilities.GetTestImagesDirectory())

    ' Make sure we got back a sheet and it's what we expect
    Assert.IsNotNull(sheet)
    AssertSheetMatchesOptions(sheet, options)

    ' Build our dictionary
    Dim spriteDict As Dictionary(Of String, Models.Sprite) = GetSpriteDictionary(sheet)

    ' Now make sure we have expected stuff.
    ' Test icon, alticon, and icon with an accent.
    AssertContainsSprite(spriteDict, "icon", Nothing, "icon_16.png", "icon_hover_16.png")
    AssertContainsSprite(spriteDict, "alticon", Nothing, "alticon_16.png", "alticon_hover_16.png")
    AssertContainsSprite(spriteDict, "icon", "accent", "icon_accent_16.png", "icon_accent_hover_16.png")

    ' Also test the non-transparent icon, as there's no hover for that.
    AssertContainsSprite(spriteDict, "icon-notrans", Nothing, "icon_notrans_16.png", Nothing)

    ' Also test the sprite that only has a hover style
    AssertContainsSprite(spriteDict, "onlyhover", Nothing, Nothing, "onlyhover_hover_16.png")

    ' We also want to ensure that our search pattern was valid, which means
    ' there can't be any sprites with GIF extensions, or even our 32x32 PNG.
    For Each sprite In sheet.Sprites

      If Not String.IsNullOrWhiteSpace(sprite.ImagePath) Then
        Assert.IsFalse(sprite.ImagePath.EndsWith(".gif"))
        Assert.IsFalse(sprite.ImagePath.EndsWith("32.png"))
      End If

      If Not String.IsNullOrWhiteSpace(sprite.HoverImagePath) Then
        Assert.IsFalse(sprite.HoverImagePath.EndsWith(".gif"))
        Assert.IsFalse(sprite.HoverImagePath.EndsWith("32.png"))
      End If

    Next

    ' Make sure there were no issues
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator performs extension filtering.")>
  Public Sub TestContentGeneration_ExtensionFiltering()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()

    ' Only detect GIFs.
    options.FileExtensions = {"gif"}

    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    Dim sheet As Models.SpriteSheet = contentGenerator.Generate(TestUtilities.GetTestImagesDirectory())

    ' Make sure we got back a sheet and it's what we expect
    Assert.IsNotNull(sheet)
    AssertSheetMatchesOptions(sheet, options)

    ' Build our dictionary
    Dim spriteDict As Dictionary(Of String, Models.Sprite) = GetSpriteDictionary(sheet)

    ' Make sure our GIFs were generated
    AssertContainsSprite(spriteDict, "icon", Nothing, "icon_16.gif", Nothing)
    AssertContainsSprite(spriteDict, "icon-notrans", Nothing, "icon_notrans_16.gif", Nothing)

    ' Ensure only GIFs were generated.
    For Each sprite In sheet.Sprites

      If Not String.IsNullOrWhiteSpace(sprite.ImagePath) Then
        Assert.IsTrue(sprite.ImagePath.EndsWith(".gif"))
      End If

      If Not String.IsNullOrWhiteSpace(sprite.HoverImagePath) Then
        Assert.IsTrue(sprite.HoverImagePath.EndsWith(".gif"))
      End If

    Next

    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator detects overlapping images.")>
  Public Sub TestContentGeneration_DetectsOverlaps()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()

    ' Only detect PNGs. This should result in an overlap
    ' between icon_16.png and icon_32.png
    options.FileExtensions = {"png"}

    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    Dim sheet As Models.SpriteSheet = contentGenerator.Generate(TestUtilities.GetTestImagesDirectory())

    ' Make sure we got back a sheet and it's what we expect
    Assert.IsNotNull(sheet)
    AssertSheetMatchesOptions(sheet, options)

    ' Make sure we still created a sprite
    Dim sprite As Models.Sprite = sheet.Sprites.FirstOrDefault(
      Function(s)
        Return s.ClassName = "icon" AndAlso
          s.HoverImagePath.EndsWith("icon_hover_16.png")
      End Function)

    Assert.IsNotNull(sprite)

    ' Make sure we have a warning
    Assert.AreEqual(1, Me.Log.WarningEntries.Count)
    Assert.IsTrue(Me.Log.ContainsWarning("already defined"))

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator detects when a file name consists only of delimiters.")>
  Public Sub TestContentGeneration_DetectsAllDelimiters()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()

    ' Scope this to a specific file, and turn all of its
    ' characters into delimiters
    options.FileSearchPattern = "icon_16.png"
    options.ClassDelimiters = {"i"c, "c"c, "o"c, "n"c, "_"c, "1"c, "6"c}

    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    Dim sheet As Models.SpriteSheet = contentGenerator.Generate(TestUtilities.GetTestImagesDirectory())

    ' Make sure we got back a sheet and it's what we expect
    Assert.IsNotNull(sheet)
    AssertSheetMatchesOptions(sheet, options)

    ' We shouldn't have created any sprites
    Assert.AreEqual(0, sheet.Sprites.Count)

    ' Make sure we have a warning
    Assert.AreEqual(1, Me.Log.WarningEntries.Count)
    Assert.IsTrue(Me.Log.ContainsWarning("No class names could be determined"))

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Content Generator Tests")>
  <Description("Tests that the content generator detects when only filtering/hover CSS classes are in a file name.")>
  Public Sub TestContentGeneration_DetectsNoRemainingClasses()

    Dim options As Models.SpriteSheetContentGeneratorOptions = TestUtilities.GetStandardOptions()

    ' Scope this to a specific file
    options.FileSearchPattern = "icon_16.png"
    options.FilterClassNames = {"icon"}

    Dim contentGenerator As Controllers.SpriteSheetContentGenerator = GetContentGenerator(options)

    Dim sheet As Models.SpriteSheet = contentGenerator.Generate(TestUtilities.GetTestImagesDirectory())

    ' Make sure we got back a sheet and it's what we expect
    Assert.IsNotNull(sheet)
    AssertSheetMatchesOptions(sheet, options)

    ' We shouldn't have created any sprites
    Assert.AreEqual(0, sheet.Sprites.Count)

    ' Make sure we have a warning
    Assert.AreEqual(1, Me.Log.WarningEntries.Count)
    Assert.IsTrue(Me.Log.ContainsWarning("consists only of valid hover and/or filter classes"))

  End Sub

  ''' <summary>
  ''' Gets a content generator instance with the specified options.
  ''' </summary>
  ''' <param name="options">The options.</param>
  ''' <returns>An instantiated content generator.</returns>
  Private Function GetContentGenerator(options As Models.SpriteSheetContentGeneratorOptions) As Controllers.SpriteSheetContentGenerator

    Dim contentGenerator As New Controllers.SpriteSheetContentGenerator(options)
    contentGenerator.Logger = Me.Log

    Return contentGenerator

  End Function

  ''' <summary>
  ''' Asserts that the properties in the provided sprite <paramref name="sheet" />
  ''' match the configured <paramref name="options" />.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  ''' <param name="options">The configured options.</param>
  Private Shared Sub AssertSheetMatchesOptions(sheet As Models.SpriteSheet,
                                               options As Models.SpriteSheetContentGeneratorOptions)

    Assert.AreEqual(options.BaseClassName, sheet.BaseClassName)
    Assert.AreEqual(options.BaseFileName, sheet.BaseFileName)
    Assert.AreEqual(options.ImageHeight, sheet.ImageDimensions.Height)
    Assert.AreEqual(options.ImageWidth, sheet.ImageDimensions.Width)

  End Sub

  ''' <summary>
  ''' Asserts that a sprite with the described properties
  ''' exists in the provided dictionary.
  ''' </summary>
  ''' <param name="spriteDict">The sprite dictionary.</param>
  ''' <param name="className">The CSS class of the sprite.</param>
  ''' <param name="filterClassName">The filtering CSS class of the sprite.</param>
  ''' <param name="imageName">The file name of the standard image.
  ''' Path information should be omitted.</param>
  ''' <param name="hoverImageName">The file name of the hover image.
  ''' Path information should be omitted.</param>
  Private Shared Sub AssertContainsSprite(spriteDict As Dictionary(Of String, Models.Sprite),
                                          className As String,
                                          filterClassName As String,
                                          imageName As String,
                                          hoverImageName As String)

    Dim key As String = Models.Sprite.GetEffectiveClassDeclaration(className, filterClassName, ".")

    ' Make sure the sprite exists
    Dim sprite As Models.Sprite = Nothing
    Assert.IsTrue(spriteDict.TryGetValue(key, sprite))

    ' Make sure the paths are equal
    If Not String.IsNullOrWhiteSpace(imageName) Then
      Assert.AreEqual(TestUtilities.GetFullImagePath(imageName), sprite.ImagePath)
    Else
      Assert.IsTrue(String.IsNullOrWhiteSpace(sprite.ImagePath))
    End If

    If Not String.IsNullOrWhiteSpace(hoverImageName) Then
      Assert.AreEqual(TestUtilities.GetFullImagePath(hoverImageName), sprite.HoverImagePath)
    Else
      Assert.IsTrue(String.IsNullOrWhiteSpace(sprite.HoverImagePath))
    End If

  End Sub

  ''' <summary>
  ''' Gets a dictionary of sprites in the provided sprite <paramref name="sheet"/>,
  ''' keyed to their <see cref="Models.Sprite.GetEffectiveClassName">effective class names</see>.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  ''' <returns>The equivalent dictionary.</returns>
  Private Shared Function GetSpriteDictionary(sheet As Models.SpriteSheet) As Dictionary(Of String, Models.Sprite)
    Return sheet.Sprites.ToDictionary(Function(s) s.GetEffectiveClassName())
  End Function

End Class