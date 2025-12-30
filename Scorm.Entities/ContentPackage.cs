using Scorm.Entities.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Scorm.Entities
{
    public class ContentPackage
    {
        public Guid Id { get; set; }
        //public int TrainingId { get; set; }    // Senin mevcut Eğitim tablonla ilişki
        public string Title { get; set; } = null!;
        public ContentStandard Standard { get; set; }
        public string FolderPath { get; set; } = null!;  // /content/packages/123
        public string LaunchPath { get; set; } = null!;  // imsmanifest / tincan'dan çıkan href/story.html
        public string? ScormVersion { get; set; }// "1.2", "2004 3rd Edition" vs.
    }
}
