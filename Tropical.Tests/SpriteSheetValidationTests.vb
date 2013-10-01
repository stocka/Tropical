Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' Contains unit tests for <see cref="Models.SpriteSheet.Validate">sprite sheet validation</see>.
''' </summary>
<TestClass()>
Public Class SpriteSheetValidationTests

  <TestMethod()>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with ""accepted"" properties and no sprites is valid.")>
  Public Sub TestEmpty()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 16
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with ""accepted"" properties and a single sprite is valid.")>
  Public Sub TestOneSprite()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 16
    End With

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
    End With
    sheet.Sprites.Add(sprite)

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with ""accepted"" properties and multiple sprites is valid.")>
  Public Sub TestMultipleSprites()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 16
    End With

    Dim spriteOne As New Models.Sprite()
    With spriteOne
      .ClassName = "sprite-class"
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
    End With
    sheet.Sprites.Add(spriteOne)

    Dim spriteTwo As New Models.Sprite()
    With spriteTwo
      .ClassName = "other-sprite-class"
      .ImagePath = "other-sprite.png"
      .HoverImagePath = "other-sprite-hover.png"
    End With
    sheet.Sprites.Add(spriteTwo)

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with a null Sprites collection is invalid.")>
  Public Sub TestNullSpritesCollection()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 16
      .Sprites = Nothing
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with a whitespace BaseClassName is invalid.")>
  Public Sub TestEmptyCssClass()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "     "
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 16
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with a BaseClassName that is not a valid CSS class is invalid.")>
  Public Sub TestInvalidCssClass()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "$BAD*CSS~MOJO"
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 16
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with a BaseFileName that is an invalid filename is invalid.")>
  Public Sub TestInvalidFileName()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "\#%*#(*&Y%#"
      .ImageHeight = 16
      .ImageWidth = 16
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with an invalid ImageHeight is invalid.")>
  Public Sub TestInvalidHeight()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageHeight = 0 ' Bad!
      .ImageWidth = 16
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Sheet Validation")>
  <Description("Tests that a sprite sheet with an invalid ImageWidth is invalid.")>
  Public Sub TestInvalidWidth()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageHeight = 16
      .ImageWidth = 0 ' Bad!
    End With

    ValidateSheet(sheet)

  End Sub

  ''' <summary>
  ''' Validates the provided sprite sheet.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  Private Shared Sub ValidateSheet(sheet As Models.SpriteSheet)

    Dim context As New ValidationContext(sheet)
    Validator.ValidateObject(sheet, context, True)

  End Sub

End Class
