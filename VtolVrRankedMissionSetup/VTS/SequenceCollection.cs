using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VtolVrRankedMissionSetup.VT;

namespace VtolVrRankedMissionSetup.VTS
{
    public class SequenceCollection
    {
        [VTInlineArray]
        public EventSequence[] Sequences => SequenceList.ToArray();


        [VTName("FOLDER_DATA")]
        [VTIgnore(VTIgnoreCondition.WhenWritingDefault)]
        public Folder[]? Folders => FolderList.Count > 0 ? FolderList.ToArray() : null;

        [VTIgnore]
        private List<EventSequence> SequenceList { get; }

        [VTIgnore]
        public List<Folder> FolderList { get; set; }

        [VTIgnore]
        public int LastOrder { get; set; }

        public SequenceCollection()
        {
            FolderList = [];
            SequenceList = [];
        }

        public EventSequence CreateSequence(string name, bool startsImmediately = true)
        {
            EventSequence sequence = new()
            {
                Id = SequenceList.Count,
                SequenceName = name,
                StartImmediately = startsImmediately,
                ListOrderIndex = LastOrder++ * 10,
            };
            SequenceList.Add(sequence);

            return sequence;
        }

        public Folder CreateFolder(string name)
        {
            Folder folder = new()
            {
                Name = name,
                SortOrder = LastOrder++ * 10,
                Expanded = false,
            };

            FolderList.Add(folder);

            return folder;
        }
    }
}
