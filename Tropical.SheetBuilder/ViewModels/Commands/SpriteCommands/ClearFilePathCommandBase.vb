Imports Tropical.Models

''' <summary>
''' A base class for clearing the file path of a sprite's image
''' (either the <see cref="Sprite.ImagePath" /> or
''' <see cref="Sprite.HoverImagePath" />).
''' </summary>
Public MustInherit Class ClearFilePathCommandBase
  Inherits SpriteCommandBase

  ''' <summary>
  ''' Initializes a new instance of the <see cref="ClearFilePathCommandBase"/> class.
  ''' </summary>
  ''' <param name="service">The sprite sheet service.</param>
  ''' <param name="canExecute">The function to be invoked as necessary
  ''' by <see cref="CanExecute" />.</param>
  ''' <param name="changed">The method to invoke after a sprite's
  ''' image has been successfully cleared. The affected sprite will be
  ''' passed to this method.</param>
  Public Sub New(service As SpriteSheetService,
                 canExecute As Func(Of Boolean),
                 changed As Action(Of Sprite))
    MyBase.New(service, canExecute, changed)
  End Sub

  ''' <summary>
  ''' Clears the image path reference specified by
  ''' <paramref name="clearValue" />.
  ''' </summary>
  ''' <param name="sprite">The sprite to update.</param>
  ''' <param name="clearValue">A reference to the value 
  ''' of the image path to clear.</param>
  Protected Sub ExecuteClearPathBase(sprite As Sprite,
                                     ByRef clearValue As String)

    If CanExecute(sprite) Then

      ' Make sure this isn't already empty.
      If Not String.IsNullOrWhiteSpace(clearValue) Then

        ' Clear it out and invoke the handler if we have one.
        clearValue = Nothing

        MyBase.TryExecuteHandler(sprite)

      End If
    End If
  End Sub

End Class
