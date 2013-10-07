Imports System.ComponentModel
Imports System.Collections.ObjectModel
Imports Tropical.Models

Public Class SpriteSheetViewModel
  Inherits BaseNotifyPropertyChanged

  Public Sub New(spriteSheet As SpriteSheet)
    Me.SpriteSheet = spriteSheet
    Me.Sprites = New ObservableCollection(Of Sprite)(spriteSheet.Sprites)
    Me.Service = New SpriteSheetService(Me.Sprites)

    ' Wire up our commands
    Me.AddCommand = New AddCommand(Me.Service,
                                   Function() Me.CanAdd,
                                   Sub(sprite)
                                     Me.CurrentSprite = sprite
                                     Me.PropagateSpriteCollectionChanges()
                                   End Sub)

    Me.DeleteCommand = New DeleteCommand(Me.Service,
                                         Function() Me.CanDelete,
                                         Sub(sprite)
                                           Me.CurrentSprite = Nothing
                                           Me.PropagateSpriteCollectionChanges()
                                         End Sub)

    Me.MoveUpCommand = New MoveUpCommand(Me.Service,
                                         Function() Me.CanMoveUp,
                                         Sub(sprite)
                                           Me.PropagateSpriteCollectionChanges()
                                         End Sub)

    Me.MoveDownCommand = New MoveDownCommand(Me.Service,
                                             Function() Me.CanMoveDown,
                                             Sub(sprite)
                                               Me.PropagateSpriteCollectionChanges()
                                             End Sub)

  End Sub

  ''' <summary>
  ''' Propagates the changes made to the <see cref="Sprites" />
  ''' collection to the <see cref="SpriteSheet">sprite sheet's</see>
  ''' <see cref="SpriteSheet.Sprites">sprites list</see>.
  ''' </summary>
  Private Sub PropagateSpriteCollectionChanges()
    Me.SpriteSheet.Sprites = Me.Sprites.ToList()
  End Sub

  Public Property SpriteSheet As SpriteSheet
  Public Property Sprites As ObservableCollection(Of Sprite)
  Public Property Service As SpriteSheetService

  Private _currentSprite As Sprite

  Public Property CurrentSprite() As Sprite
    Get
      Return _currentSprite
    End Get
    Set(value As Sprite)

      If value IsNot _currentSprite Then

        ' We're skipping CanAdd here because it's
        ' always true.
        RaisePropertyChanging(Function() CurrentSprite)
        RaisePropertyChanging(Function() CanDelete)
        RaisePropertyChanging(Function() CanMoveUp)
        RaisePropertyChanging(Function() CanMoveDown)

        _currentSprite = value

        RaisePropertyChanged(Function() CurrentSprite)
        RaisePropertyChanged(Function() CanDelete)
        RaisePropertyChanged(Function() CanMoveUp)
        RaisePropertyChanged(Function() CanMoveDown)

      End If

    End Set
  End Property

  ''' <summary>
  ''' Handles when a property has changed.  Used to simplify the process
  ''' of invoking <see cref="ICommand.CanExecuteChanged" /> when corresponding
  ''' properties (such as <see cref="CanDelete" />) are changed.
  ''' </summary>
  ''' <param name="sender">The sender.</param>
  ''' <param name="e">The <see cref="PropertyChangedEventArgs"/> instance containing the event data.</param>
  Private Sub PropertyChangedHandler(sender As Object, e As PropertyChangedEventArgs) Handles Me.PropertyChanged

    Select Case e.PropertyName.ToLower()

      Case "canadd"
        AddCommand.RaiseCanExecuteChanged()

      Case "candelete"
        DeleteCommand.RaiseCanExecuteChanged()

      Case "canmoveup"
        MoveUpCommand.RaiseCanExecuteChanged()

      Case "canmovedown"
        MoveDownCommand.RaiseCanExecuteChanged()

    End Select

  End Sub

#Region "Command Implementation"

  Public ReadOnly Property CanAdd() As Boolean
    Get
      ' Right now, we can always add sprites to the sheet
      Return True
    End Get
  End Property

  Public ReadOnly Property CanDelete() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  Public ReadOnly Property CanMoveUp() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  Public ReadOnly Property CanMoveDown() As Boolean
    Get
      Return Me.CurrentSprite IsNot Nothing
    End Get
  End Property

  Public Property AddCommand As AddCommand
  Public Property DeleteCommand As DeleteCommand
  Public Property MoveUpCommand As MoveUpCommand
  Public Property MoveDownCommand As MoveDownCommand

#End Region

End Class
