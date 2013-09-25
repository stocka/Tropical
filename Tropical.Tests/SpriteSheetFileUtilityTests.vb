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

#Region "Save/Load Tests"

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

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to save a sprite sheet with a null/whitespace file path.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub SaveSheet_NullFilePath()
    Controllers.SpriteSheetFileUtilities.SaveSpriteSheet("   ", GetSheetInstance(), Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to save a sprite sheet with an invalid file path.")>
  <ExpectedException(GetType(ArgumentException), AllowDerivedTypes:=False)>
  Public Sub SaveSheet_InvalidFilePath()
    Controllers.SpriteSheetFileUtilities.SaveSpriteSheet("CON.xml", GetSheetInstance(), Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to save a sprite sheet with an invalid directory path.")>
  <ExpectedException(GetType(ArgumentException), AllowDerivedTypes:=False)>
  Public Sub SaveSheet_InvalidDirectoryPath()
    Controllers.SpriteSheetFileUtilities.SaveSpriteSheet("|directory\sheet.xml", GetSheetInstance(), Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to save a sprite sheet to a file that cannot be written to.")>
  Public Sub SaveSheet_InaccessibleFile()

    Dim filePath As String = Path.GetTempFileName()

    ' Get an exclusive write handle, which should lock out
    ' the save.
    Using FileStream As FileStream = File.Create(filePath)
      ' We expect this to fail.
      Assert.IsFalse(Controllers.SpriteSheetFileUtilities.SaveSpriteSheet(filePath, GetSheetInstance(), Me.Log))
    End Using

    ' Make sure we have stuff in the log.
    Assert.AreEqual(1, Log.ErrorEntries.Count)
    Assert.IsTrue(Log.ContainsError("could not be created"))

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to save a null sprite sheet.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub SaveSheet_NullSheet()
    Controllers.SpriteSheetFileUtilities.SaveSpriteSheet(Path.GetRandomFileName(), Nothing, Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to save a sprite sheet with a null logger.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub SaveSheet_NullLogger()
    Controllers.SpriteSheetFileUtilities.SaveSpriteSheet(Path.GetRandomFileName(), GetSheetInstance(), Nothing)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet with a null/whitespace file path.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub LoadSheet_NullFilePath()
    Controllers.SpriteSheetFileUtilities.LoadSpriteSheet("   ", Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet with an invalid file path.")>
  <ExpectedException(GetType(ArgumentException), AllowDerivedTypes:=False)>
  Public Sub LoadSheet_InvalidFilePath()
    Controllers.SpriteSheetFileUtilities.LoadSpriteSheet("CON.xml", Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet with an invalid directory path.")>
  <ExpectedException(GetType(ArgumentException), AllowDerivedTypes:=False)>
  Public Sub LoadSheet_InvalidDirectoryPath()
    Controllers.SpriteSheetFileUtilities.LoadSpriteSheet("|directory\sheet.xml", Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet from a nonexistent file.")>
  <ExpectedException(GetType(FileNotFoundException))>
  Public Sub LoadSheet_InaccessibleFile()
    ' Generate a random GUID to make a nonexistent file name.
    Controllers.SpriteSheetFileUtilities.LoadSpriteSheet(Guid.NewGuid.ToString(), Me.Log)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet with a null logger.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub LoadSheet_NullLogger()
    Controllers.SpriteSheetFileUtilities.LoadSpriteSheet(Path.GetRandomFileName(), Nothing)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet from an empty file.")>
  Public Sub LoadSheet_EmptyFile()

    ' Get a new file
    Dim emptyFilePath As String = Path.GetTempFileName()

    ' We expect this to fail.
    Assert.IsNull(Controllers.SpriteSheetFileUtilities.LoadSpriteSheet(emptyFilePath, Me.Log))

    ' Check the error log.
    Assert.AreEqual(1, Log.ErrorEntries.Count)
    Assert.IsTrue(Me.Log.ContainsExactError("An error occured during deserialization."))

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to load a sprite sheet from a file with just an XML declaration.")>
  Public Sub LoadSheet_EmptyXmlFile()

    ' Get a new file
    Dim emptyFilePath As String = Path.GetRandomFileName()
    File.WriteAllText(emptyFilePath, "<xml></xml>")

    ' We expect this to fail.
    Assert.IsNull(Controllers.SpriteSheetFileUtilities.LoadSpriteSheet(emptyFilePath, Me.Log))

    ' Check the error log.
    Assert.AreEqual(1, Log.ErrorEntries.Count)
    Assert.IsTrue(Me.Log.ContainsExactError("An error occured during deserialization."))

  End Sub

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

    ' We can ignore case for file names.
    Assert.AreEqual(expectedSheet.BaseFileName, actualSheet.BaseFileName, True)
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

      ' We can ignore case for paths.
      Assert.AreEqual(expectedSprite.ImagePath, actualSprite.ImagePath, True)
      Assert.AreEqual(expectedSprite.HoverImagePath, actualSprite.HoverImagePath, True)

    Next

    ' Now make sure we don't have any extra sprites
    For Each actualSprite In actualSprites

      If Not expectedSpriteSet.Contains(actualSprite.ID) Then
        Assert.Fail("The sprite with ID " & actualSprite.ID.ToString() & " does not match any of the expected sprites.")
      End If

    Next

  End Sub

#End Region

#Region "Image Path Relocation Tests"

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Relocates the image paths in a sprite sheet that contains no sprites.")>
  Public Sub RelocateImagePaths_NoSprites()

    ' Set up our sheet
    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' Pass the rest off to the internal method.
    RelocateSpritePathsInternal(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Relocates the image paths in a sprite sheet that contains a single sprite.")>
  Public Sub RelocateImagePaths_OneSprite()

    ' Set up our sheet
    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' Add a single sprite.
    TestUtilities.AddSprite(sheet, "icon", "filter-class", "image.png", "image-hover.png")

    ' Pass the rest off to the internal method.
    RelocateSpritePathsInternal(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Relocates the image paths in a sprite sheet that contains several sprites.")>
  Public Sub RelocateImagePaths_ManySprites()

    ' Set up our sheet
    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' Add multiple sprites with different permutations
    ' of filter classes and file names. To be tricky, add one
    ' with absolutely no paths.
    TestUtilities.AddSprite(sheet, "icon", "filter-class", "image.png", "image-hover.png")
    TestUtilities.AddSprite(sheet, "icon2", Nothing, Nothing, "image2-hover.png")
    TestUtilities.AddSprite(sheet, "icon3", "", "image3.png", Nothing)
    TestUtilities.AddSprite(sheet, "icon4", Nothing, Nothing, Nothing)

    ' Pass the rest off to the internal method.
    RelocateSpritePathsInternal(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to relocate the image paths in a null sprite sheet.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub RelocateImagePaths_NullSheet()
    RelocateSpritePathsInternal(Nothing)
  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to relocate the image paths in a sprite sheet with a null sprite collection.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub RelocateImagePaths_NullSpriteCollection()

    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    ' This should cause an error.
    sheet.Sprites = Nothing
    RelocateSpritePathsInternal(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to relocate the image paths to a null/whitespace directory.")>
  <ExpectedException(GetType(ArgumentNullException))>
  Public Sub RelocateImagePaths_NullDirectory()

    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    Controllers.SpriteSheetFileUtilities.RelocateImagePaths(sheet, "    ")

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet File Utilities")>
  <Description("Attempts to relocate the image paths to an invalid directory.")>
  <ExpectedException(GetType(ArgumentException), AllowDerivedTypes:=False)>
  Public Sub RelocateImagePaths_InvalidDirectory()

    Dim sheet As Models.SpriteSheet = GetSheetInstance()

    Controllers.SpriteSheetFileUtilities.RelocateImagePaths(sheet, "|BAD$DIRECTORY*MOJO")

  End Sub

  ''' <summary>
  ''' Relocates images in a sprite sheet, after which it validates
  ''' that they have been properly relocated.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet to relocate.</param>
  Private Sub RelocateSpritePathsInternal(sheet As Models.SpriteSheet)

    ' Get our directory - temp will do.
    Dim tempDirectory As String = Path.GetTempPath()

    ' Keep track of things that had image paths and hover image paths
    Dim imagePaths As New HashSet(Of Guid)
    Dim hoverImagePaths As New HashSet(Of Guid)

    ' Make sure our sheet and sprites collection isn't null - they might be
    ' if we're testing error paths
    If sheet IsNot Nothing AndAlso sheet.Sprites IsNot Nothing Then
      ' Build hashes of our sprites.
      imagePaths = New HashSet(Of Guid)(
        sheet.Sprites.Where(Function(s)
                              Return Not String.IsNullOrWhiteSpace(s.ImagePath)
                            End Function).Select(Function(s) s.ID))

      hoverImagePaths = New HashSet(Of Guid)(
        sheet.Sprites.Where(Function(s)
                              Return Not String.IsNullOrWhiteSpace(s.HoverImagePath)
                            End Function).Select(Function(s) s.ID))
    End If

    ' Relocate the image paths.
    Controllers.SpriteSheetFileUtilities.RelocateImagePaths(sheet, tempDirectory)

    ' Now iterate over each of our sprites, and make sure they were relocated
    For Each sprite In sheet.Sprites

      ' If our image path is defined, make sure it was relocated
      If imagePaths.Contains(sprite.ID) Then
        TestUtilities.AssertDirectoriesEqual(tempDirectory, Path.GetDirectoryName(sprite.ImagePath))
      End If

      ' If our hover image path is defined, make sure it was relocated
      If hoverImagePaths.Contains(sprite.ID) Then
        TestUtilities.AssertDirectoriesEqual(tempDirectory, Path.GetDirectoryName(sprite.HoverImagePath))
      End If

    Next

  End Sub

#End Region

#Region "Helper Methods/Properties"

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

#End Region

End Class