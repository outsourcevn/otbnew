//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebTMDT.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class ImageProduct
    {
        public long F1 { get; set; }
        public Nullable<long> F2 { get; set; }
        public string F3 { get; set; }
    
        public virtual Product Product { get; set; }
    }
}