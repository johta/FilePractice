using PropertyChanged;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows.Input;
using PCLStorage;

namespace FilePractice
{
    [AddINotifyPropertyChangedInterface]
    class MainPageViewModel
    {
        //ローカルプロパティの宣言
        public ICommand CreateCommand { get; }
        public ICommand DeleteCommand { get; }
        public ObservableCollection<String> Files { get; set; }
        public string FileText { get; set; }


        /// <summary>
        /// コンストラクタ
        /// </summary>
        public MainPageViewModel()
        {
            //コマンドを初期化
            CreateCommand = new CreateCommand(CreateFile);
            DeleteCommand = new DeleteCommand(DeleteFiles);

            //表示のリフレッシュ
            Initialize();
        }

        /// <summary>
        /// ファイル作成と表示のリフレッシュ
        /// </summary>
        private async void CreateFile()
        {
            var localStorage = FileSystem.Current.LocalStorage;
            var time = DateTime.Now.ToString("yyyyMMddHHmmss");
            var filename = time + ".txt";
            var file = await localStorage.CreateFileAsync(filename, CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync("Date:"+time);
            Initialize();
        }

        /// <summary>
        /// ファイル全消去と表示のリフレッシュ
        /// </summary>
        private async void DeleteFiles()
        {
            var localStorage = FileSystem.Current.LocalStorage;
            var files = localStorage.GetFilesAsync().Result;
            foreach (var file in files)
            {
               await file.DeleteAsync();
            }
            Initialize();
        }

        private async void Initialize()
        {
            var localStorage = FileSystem.Current.LocalStorage;
            var files = localStorage.GetFilesAsync().Result;
            Files = new ObservableCollection<string>() { };

            foreach (var file in files)
            {
                Files.Add(file.Name);
            }
        }
    }

    internal class CreateCommand : ICommand
    {
        private readonly Action _action;

        internal CreateCommand(Action action)
        {
            this._action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this._action();
        }
    }
    internal class DeleteCommand : ICommand
    {
        private readonly Action _action;

        internal DeleteCommand(Action action)
        {
            this._action = action;
        }

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            this._action();
        }
    }
}
