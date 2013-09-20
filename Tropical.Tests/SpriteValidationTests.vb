Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

<TestClass()>
Public Class SpriteValidationTests

  <TestMethod()>
  Public Sub TestValid()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  Public Sub TestValid_NoImagePath()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  Public Sub TestValid_NoHoverPath()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .ImagePath = "sprite.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestEmptyCssClass()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "       "
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidCssClass()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "$BAD*CSS*MOJO"
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidFilePath()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .ImagePath = "CON.png"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  Public Sub TestInvalidHoverFilePath()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .ImagePath = "sprite.png"
      .HoverImagePath = "NUL"
    End With

    ValidateSprite(sprite)

  End Sub

  ''' <summary>
  ''' Validates the provided sprite.
  ''' </summary>
  ''' <param name="sprite">The sprite.</param>
  Private Sub ValidateSprite(sprite As Models.Sprite)

    Dim context As New ValidationContext(sprite)
    Validator.ValidateObject(sprite, context, True)

  End Sub

End Class
