﻿Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteSheetValidationTests

  <TestMethod()>
  Public Sub TestEmpty()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  Public Sub TestOneSprite()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
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
  Public Sub TestMultipleSprites()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
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
  Public Sub TestNullSpritesCollection()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
      .Sprites = Nothing
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestEmptyCssClass()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "     "
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidCssClass()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "$BAD*CSS~MOJO"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidFileName()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "\#%*#(*&Y%#"
      .ImageDimensions = New System.Drawing.Size(16, 16)
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidHeight()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(16, 0) ' Bad!
    End With

    ValidateSheet(sheet)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidWidth()

    Dim sheet As New Models.SpriteSheet()
    With sheet
      .BaseClassName = "td-icon"
      .BaseFileName = "td-icons"
      .ImageDimensions = New System.Drawing.Size(0, 16) ' Bad!
    End With

    ValidateSheet(sheet)

  End Sub

  ''' <summary>
  ''' Validates the provided sprite sheet.
  ''' </summary>
  ''' <param name="sheet">The sprite sheet.</param>
  Private Sub ValidateSheet(sheet As Models.SpriteSheet)

    Dim context As New ValidationContext(sheet)
    Validator.ValidateObject(sheet, context, True)

  End Sub

End Class