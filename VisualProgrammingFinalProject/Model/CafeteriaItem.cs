//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VisualProgrammingFinalProject.Model
{
    using System;
    using System.Collections.Generic;
    
    public partial class CafeteriaItem
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public CafeteriaItem()
        {
            this.CafeteriaPurchases = new HashSet<CafeteriaPurchas>();
            this.ParentRestrictions = new HashSet<ParentRestriction>();
        }
    
        public int ItemID { get; set; }
        public string ItemName { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<CafeteriaPurchas> CafeteriaPurchases { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ParentRestriction> ParentRestrictions { get; set; }
    }
}
