Imports System.IO
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' Contains unit tests for <see cref="Controllers.SpriteSheetFileUtilities" /> methods.
''' </summary>
<TestClass()>
Public Class SpriteSheetFileUtilityTests

  ''' <summary>
  ''' The log used for any file utility testing.
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
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Saves and loads a sprite sheet that contains no sprites.")>
  Public Sub SaveAndLoadSheet_NoSprites()

    ' Set up our sheet
    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' Now pass off the rest to the internal method.
    SaveLoadSheetInternal(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Saves and loads a sprite sheet that contains one sprite.")>
  Public Sub SaveAndLoadSheet_OneSprite()

    ' Set up our sheet
    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' Add a single sprite.
    TestUtilities.AddSprite(sheet, "icon", "filter-class", "image.png", "image-hover.png")

    ' Now pass off the rest to the internal method.
    SaveLoadSheetInternal(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Saves and loads a sprite sheet that contains several sprites.")>
  Public Sub SaveAndLoadSheet_ManySprites()

    ' Set up our sheet
    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' Add multiple sprites with different permutations
    ' of filter classes and file names.
    TestUtilities.AddSprite(sheet, "icon", "filter-class", "image.png", "image-hover.png")
    TestUtilities.AddSprite(sheet, "icon2", Nothing, Nothing, "image2-hover.png")
    TestUtilities.AddSprite(sheet, "icon3", "", "image3.png", Nothing)

    ' Now pass off the rest to the internal method.
    SaveLoadSheetInternal(sheet)

  End Sub

  ''' <summary>
  ''' Gets a sprite sheet instance.
  ''' </summary>
  ''' <returns>An instantiated sprite sheet.</returns>
  Private Shared Function GetSheetInstance() As Models.SpriteSheet

    ' Set up our sheet
    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    Return sheet

  End Function

  ''' <summary>
  ''' Saves and loads a sprite sheet, after which it validates
  ''' that the original sprite sheet and the loaded instance
  ''' are equivalent.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet to save.</param>
  Private Sub SaveLoadSheetInternal(sheet As Models.SpriteSheet)

    ' Get a temp file path
    Dim tempSpritePath As String = Path.GetTempFileName()

    ' Make sure the sprite sheet saved.
    Assert.IsTrue(Controllers.SpriteSheetFileUtilities.SaveSpriteSheet(tempSpritePath, sheet, Me.Log))

    ' Now re-open the file.
    Dim loadedSheet As Models.SpriteSheet =
      Controllers.SpriteSheetFileUtilities.LoadSpriteSheet(tempSpritePath, Me.Log)

    ' Make sure it's not null
    Assert.IsNotNull(loadedSheet)

    ' Now make sure the sheets are the same.
    AssertSpriteSheetsEqual(sheet, loadedSheet)

    ' Make sure the log is empty
    TestUtilities.AssertNoWarningsOrErrors(Me.Log)

  End Sub

  ''' <summary>
  ''' Asserts that two sprite sheet instances are equivalent.
  ''' </summary>
  ''' <param name="expectedSheet">The expected sprite sheet.</param>
  ''' <param name="actualSheet">The actual sprite sheet.</param>
  Private Shared Sub AssertSpriteSheetsEqual(expectedSheet As Models.SpriteSheet,
                                             actualSheet As Models.SpriteSheet)

    ' Make sure the actual sheet isn't null.
    Assert.IsNotNull(actualSheet)

    ' Validate basic sheet properties.
    Assert.AreEqual(expectedSheet.BaseClassName, actualSheet.BaseClassName)
    Assert.AreEqual(expectedSheet.BaseFileName, actualSheet.BaseFileName)
    Assert.AreEqual(expectedSheet.ImageDimensions, actualSheet.ImageDimensions)

    ' Now make sure the sprite collections are the same
    AssertSpritesEqual(expectedSheet.Sprites, actualSheet.Sprites)

  End Sub

  ''' <summary>
  ''' Asserts that two sprite collections are equivalent.
  ''' </summary>
  ''' <param name="expectedSprites">The expected sprite collection.</param>
  ''' <param name="actualSprites">The actual sprite collection.</param>
  Private Shared Sub AssertSpritesEqual(expectedSprites As List(Of Models.Sprite),
                                        actualSprites As List(Of Models.Sprite))

    ' Make sure the sprites collection isn't null.
    Assert.IsNotNull(actualSprites)

    ' Make sure the counts are the same.
    Assert.AreEqual(expectedSprites.Count, actualSprites.Count)

    ' Keep track of all sprites we've found
    Dim expectedSpriteSet As New HashSet(Of Guid)

    For Each expectedSprite In expectedSprites

      ' Add our ID to the list of ones we expect.
      expectedSpriteSet.Add(expectedSprite.ID)

      ' Find the equivalent actual sprite
      Dim actualSprite As Models.Sprite = actualSprites.FirstOrDefault(Function(s) s.ID = expectedSprite.ID)

      ' Make sure it exists.
      If actualSprite Is Nothing Then
        Assert.Fail("The sprite with ID " & expectedSprite.ID.ToString() & " was not found in the collection.")
      End If

      ' Now make sure they're equal
      Assert.AreEqual(expectedSprite.ClassName, actualSprite.ClassName)
      Assert.AreEqual(expectedSprite.FilterClassName, actualSprite.FilterClassName)
      Assert.AreEqual(expectedSprite.ImagePath, actualSprite.ImagePath)
      Assert.AreEqual(expectedSprite.HoverImagePath, actualSprite.HoverImagePath)

    Next

    ' Now make sure we don't have any extra sprites
    For Each actualSprite In actualSprites

      If Not expectedSpriteSet.Contains(actualSprite.ID) Then
        Assert.Fail("The sprite with ID " & actualSprite.ID.ToString() & " does not match any of the expected sprites.")
      End If

    Next

  End Sub

End Class