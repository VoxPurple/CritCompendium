using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using CritCompendium.Business;
using CritCompendiumInfrastructure;
using CritCompendiumInfrastructure.Models;
using CritCompendiumInfrastructure.Business;

namespace CritCompendium.ViewModels.ObjectViewModels
{
   public sealed class RandomTableViewModel : NotifyPropertyChanged
   {
      #region Fields

      private readonly DiceService _diceService = DependencyResolver.Resolve<DiceService>();

      private readonly RandomTableModel _randomTableModel;
      private readonly List<RandomTableRowViewModel> _rows = new List<RandomTableRowViewModel>();
      private readonly BackgroundWorker _rollWorker = new BackgroundWorker();

      private bool _isRolling = false;
      private int _rolledNumber = 0;
      private string _rolledValue = "No Table Row Selected";

      private readonly ICommand _rollCommand;
      private readonly ICommand _copyValueCommand;
      private readonly ICommand _selectRowCommand;

      #endregion

      #region Constructor

      /// <summary>
      /// Creates a new instance of <see cref="RandomTableViewModel"/>
      /// </summary>
      public RandomTableViewModel(RandomTableModel randomTableModel)
      {
         _randomTableModel = randomTableModel;

         foreach (RandomTableRowModel rowModel in _randomTableModel.Rows)
         {
            _rows.Add(new RandomTableRowViewModel(rowModel));
         }

         _rollWorker.DoWork += _rollWorker_DoWork;

         _rollCommand = new RelayCommand(obj => true, obj => Roll());
         _copyValueCommand = new RelayCommand(obj => true, obj => CopyValue(obj as RandomTableRowViewModel));
         _selectRowCommand = new RelayCommand(obj => true, obj => SelectRow(obj as RandomTableRowViewModel));
      }

      #endregion

      #region Properties

      /// <summary>
      /// Gets random table model
      /// </summary>
      public RandomTableModel RandomTableModel
      {
         get { return _randomTableModel; }
      }

      /// <summary>
      /// Gets name
      /// </summary>
      public string Name
      {
         get { return !String.IsNullOrWhiteSpace(_randomTableModel.Name) ? _randomTableModel.Name : "Unknown Name"; }
      }

      /// <summary>
      /// Gets tags
      /// </summary>
      public string Tags
      {
         get { return String.Empty; }
      }

      /// <summary>
      /// Gets die
      /// </summary>
      public string Die
      {
         get { return _randomTableModel.Die; }
      }

      /// <summary>
      /// Gets header
      /// </summary>
      public string Header
      {
         get { return _randomTableModel.Header; }
      }

      /// <summary>
      /// Gets rolled number
      /// </summary>
      public int RolledNumber
      {
         get { return _rolledNumber; }
         set { Set(ref _rolledNumber, value); }
      }

      /// <summary>
      /// Gets rolled value
      /// </summary>
      public string RolledValue
      {
         get { return _rolledValue; }
         set { Set(ref _rolledValue, value); }
      }

      /// <summary>
      /// Gets rwos
      /// </summary>
      public IEnumerable<RandomTableRowViewModel> Rows
      {
         get { return _rows; }
      }

      /// <summary>
      /// Gets roll command
      /// </summary>
      public ICommand RollCommand
      {
         get { return _rollCommand; }
      }

      /// <summary>
      /// Gets copy value command
      /// </summary>
      public ICommand CopyValueCommand
      {
         get { return _copyValueCommand; }
      }

      public ICommand SelectRowCommand
      {
         get { return _selectRowCommand; }
      }

      #endregion

      #region Private Methods

      private void Roll()
      {
         if (_isRolling)
         {
            return;
         }

         _rollWorker.RunWorkerAsync();

         //foreach (RandomTableRowViewModel rowView in _rows)
         //{
         //    if (rowView.Selected == true)
         //    {
         //        rowView.Selected = false;
         //        break;
         //    }
         //}

         //string dieString = !String.IsNullOrWhiteSpace(_randomTableModel.Die) ? _randomTableModel.Die.Replace("d", String.Empty) : "2";
         //if (Int32.TryParse(dieString, out int die))
         //{
         //    _rolledValue = null;
         //    _rolledNumber = _diceService.RandomNumber(1, die);
         //    foreach (RandomTableRowViewModel rowView in _rows)
         //    {
         //        if (_rolledNumber >= rowView.Min && _rolledNumber <= rowView.Max)
         //        {
         //            _rolledValue = rowView.Value;
         //            rowView.Selected = true;
         //            break;
         //        }
         //    }
         //}
      }

      private void SelectRow(RandomTableRowViewModel rowView)
      {
         if (rowView != null)
         {
            bool isSelected = rowView.Selected;
            _clearSelectedRows();
            if (isSelected)
            {
               return;
            }
            rowView.Selected = !rowView.Selected;
            RolledNumber = rowView.Min;
            RolledValue = rowView.Value;
         }
      }

      private void CopyValue(RandomTableRowViewModel rowView)
      {
         if (rowView != null && !String.IsNullOrWhiteSpace(rowView.Value))
         {
            Clipboard.SetText(rowView.Value);
         }
      }

      private void _clearSelectedRows()
      {
         foreach (RandomTableRowViewModel rowView in _rows)
         {
            if (rowView.Selected == true)
            {
               rowView.Selected = false;
            }
         }
         RolledNumber = 0;
         RolledValue = "No Table Row Selected";
      }

      private void _rollWorker_DoWork(object sender, DoWorkEventArgs e)
      {
         _isRolling = true;
         int rolls = 0;
         while (++rolls < 10)
         {
            _clearSelectedRows();

            string dieString = !String.IsNullOrWhiteSpace(_randomTableModel.Die) ? _randomTableModel.Die.Replace("d", String.Empty) : "2";
            if (Int32.TryParse(dieString, out int die))
            {
               RolledValue = "No row selected";
               RolledNumber = _diceService.RandomNumber(1, die);
               foreach (RandomTableRowViewModel rowView in _rows)
               {
                  if (_rolledNumber >= rowView.Min && _rolledNumber <= rowView.Max)
                  {
                     RolledValue = rowView.Value;
                     rowView.Selected = true;
                     break;
                  }
               }
            }

            Thread.Sleep(100);
         }
         _isRolling = false;
      }

      #endregion
   }
}
