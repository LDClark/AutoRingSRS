using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;

namespace AutoRingUAB
{
    public class MainViewModel : ViewModelBase
    {
        private readonly IEsapiService _esapiService;
        private readonly IDialogService _dialogService;
        public MainViewModel(IEsapiService esapiService, IDialogService dialogService)
        {
            _esapiService = esapiService;
            _dialogService = dialogService;
        }
        private Struct[] _structures;
        public Struct[] Structures
        {
            get => _structures;
            set => Set(ref _structures, value);
        }
        private Struct[] _structuresRingInner;
        public Struct[] StructuresRingInner
        {
            get => _structuresRingInner;
            set => Set(ref _structuresRingInner, value);
        }
        private Struct[] _structuresRingMiddle;
        public Struct[] StructuresRingMiddle
        {
            get => _structuresRingMiddle;
            set => Set(ref _structuresRingMiddle, value);
        }
        private Struct[] _structuresRingOuter;
        public Struct[] StructuresRingOuter
        {
            get => _structuresRingOuter;
            set => Set(ref _structuresRingOuter, value);
        }
        private StructSet[] _structureSets;
        public StructSet[] StructureSets
        {
            get => _structureSets;
            set => Set(ref _structureSets, value);
        }
        private StructSet _selectedStructureSet;
        public StructSet SelectedStructureSet
        {
            get => _selectedStructureSet;
            set => Set(ref _selectedStructureSet, value);
        }
        //private Struct[] _selectedStructures;
        //public Struct[] SelectedStructures
        //{
        //    get => _selectedStructures;
        //    set => Set(ref _selectedStructures, value);
        //}
        private Struct _selectedStructureRingInner;
        public Struct SelectedStructureRingInner
        {
            get => _selectedStructureRingInner;
            set => Set(ref _selectedStructureRingInner, value);
        }
        private Struct _selectedStructureRingMiddle;
        public Struct SelectedStructureRingMiddle
        {
            get => _selectedStructureRingMiddle;
            set => Set(ref _selectedStructureRingMiddle, value);
        }
        private Struct _selectedStructureRingOuter;
        public Struct SelectedStructureRingOuter
        {
            get => _selectedStructureRingOuter;
            set => Set(ref _selectedStructureRingOuter, value);
        }
        public ICommand StartCommand => new RelayCommand(Start);
        public ICommand GetStructuresCommand => new RelayCommand(GetStructures);
        public ICommand GetRingsCommand => new RelayCommand(CreateRings);
        private async void Start()
        {
            StructureSets = await _esapiService.GetStructureSetsAsync();
        }

        private async void GetStructures()
        {
            Structures = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "TV");
            StructuresRingInner = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Inner");
            StructuresRingMiddle = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Middle");
            StructuresRingOuter = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Outer");
        }

        private async void CreateRings()
        {
            ObservableCollection<string> ptvIds = new ObservableCollection<string>();
            string selectedStructureSetId = SelectedStructureSet?.StructureSetId;
            string ringInnerId = SelectedStructureRingInner?.StructureId;
            string ringMiddleId = SelectedStructureRingMiddle?.StructureId;
            string ringOuterId = SelectedStructureRingOuter?.StructureId;

            foreach (var s in Structures)
            {
                if (s.IsSelected == true)
                {
                    ptvIds.Add(s.StructureId);
                }
            }

            if (ringInnerId == "<Create new structure>")
                ringInnerId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring_Inner");                      
            if (ringMiddleId == "<Create new structure>")
                ringMiddleId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring_Middle");
            if (ringOuterId == "<Create new structure>")
                ringOuterId = await _esapiService.GetEditableRingNameAsync(selectedStructureSetId, "Ring_Outer");

            _dialogService.ShowProgressDialog("Adding rings...",
                async progress =>
                {
                    await _esapiService.AddRingAsync(selectedStructureSetId, ptvIds, ringInnerId, ringMiddleId, ringOuterId);
                    StructuresRingInner = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Inner");
                    StructuresRingMiddle = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Middle");
                    StructuresRingOuter = await _esapiService.GetStructureIdsAsync(SelectedStructureSet.StructureSetId, "Ring_Outer");
                });
        }
    }
}
