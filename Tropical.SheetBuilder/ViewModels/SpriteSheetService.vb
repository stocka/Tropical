Imports System.Collections.ObjectModel
Imports Tropical.Models

Public Class SpriteSheetService

  Private ReadOnly _sprites As ObservableCollection(Of Sprite)

  Public Sub New(sprites As ObservableCollection(Of Sprite))
    _sprites = sprites
  End Sub

  Public Function AddSprite(insertAtSprite As Sprite) As Sprite

    Dim createdSprite As New Sprite()
    Dim insertIndex As Int32 = -1

    If insertAtSprite IsNot Nothing Then
      insertIndex = _sprites.IndexOf(insertAtSprite)
    End If

    ' If we didn't find the sprite (or didn't have one), insert it at the end
    If insertIndex >= 0 Then
      _sprites.Insert(insertIndex, createdSprite)
    Else
      _sprites.Add(createdSprite)
    End If

    Return createdSprite

  End Function

  Public Function DeleteSprite(sprite As Sprite) As Boolean
    Return _sprites.Remove(sprite)
  End Function

  Public Function MoveSprite(sprite As Sprite, moveUp As Boolean) As Boolean

    Dim oldSpriteIndex As Int32
    Dim newSpriteIndex As Int32

    If sprite IsNot Nothing Then

      ' Find the sprite
      oldSpriteIndex = _sprites.IndexOf(sprite)

      If oldSpriteIndex <> -1 Then

        ' Figure out the new sprite index
        If moveUp Then
          newSpriteIndex = oldSpriteIndex - 1
        Else
          newSpriteIndex = oldSpriteIndex + 1
        End If

        ' Keep the new index within the bounds of the collection
        newSpriteIndex = Math.Min(_sprites.Count - 1, Math.Max(0, newSpriteIndex))

        ' If they differ, move and return true.
        If newSpriteIndex <> oldSpriteIndex Then
          _sprites.Move(oldSpriteIndex, newSpriteIndex)
          Return True
        End If

      End If

    End If

    ' Something didn't work out.
    Return False

  End Function

End Class
