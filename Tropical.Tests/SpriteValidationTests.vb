Imports System.ComponentModel.DataAnnotations
Imports System.Text
Imports Microsoft.VisualStudio.TestTools.UnitTesting

''' <summary>
''' Contains unit tests for <see cref="Models.Sprite.Validate">sprite validation</see>.
''' </summary>
<TestClass()>
Public Class SpriteValidationTests

  <TestMethod()>
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with ""accepted"" properties is valid.")>
  Public Sub TestValid()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
      .FilterClassName = "accent"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with no ImagePath is valid.")>
  Public Sub TestValid_NoImagePath()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with no HoverImagePath is valid.")>
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
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with a whitespace ClassName is invalid.")>
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
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with a ClassName that is not a valid CSS class is invalid.")>
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
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with a FilterClassName that is not a valid CSS class is invalid.")>
  Public Sub TestInvalidFilterCssClass()

    Dim sprite As New Models.Sprite()
    With sprite
      .ClassName = "sprite-class"
      .FilterClassName = "$BAD*CSS*MOJO"
      .ImagePath = "sprite.png"
      .HoverImagePath = "sprite-hover.png"
    End With

    ValidateSprite(sprite)

  End Sub

  <TestMethod()>
  <ExpectedException(GetType(ValidationException))>
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with an ImagePath that is an invalid filename is invalid.")>
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
  <TestCategory("Sprite Validation")>
  <Description("Tests that a sprite with a HoverImagePath with an invalid filename is invalid.")>
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
  Private Shared Sub ValidateSprite(sprite As Models.Sprite)

    Dim context As New ValidationContext(sprite)
    Validator.ValidateObject(sprite, context, True)

  End Sub

End Class
