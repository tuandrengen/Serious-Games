//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace P2SeriousGame.SQL
{
    using System;
    using System.Collections.Generic;
    
    public partial class ForeignKeys
    {
        public int Id { get; set; }
        public int PersonId { get; set; }
        public int SessionId { get; set; }
        public int RoundsId { get; set; }
    
        public virtual Person Person { get; set; }
        public virtual Rounds Rounds { get; set; }
        public virtual Session Session { get; set; }
    }
}
