//------------------------------------------------------------------------------
// <auto-generated>
//     此代码已从模板生成。
//
//     手动更改此文件可能导致应用程序出现意外的行为。
//     如果重新生成代码，将覆盖对此文件的手动更改。
// </auto-generated>
//------------------------------------------------------------------------------

namespace Entities
{
    using System;
    using System.Collections.Generic;
    
    public partial class Previlege : IEntity
    {
        public int Id { get; set; }
        public int RoleId { get; set; }
        public int ControllerId { get; set; }
        public Nullable<int> ActionId { get; set; }
        public bool IsDeleted { get; set; }
        public byte[] Ver { get; set; }
        public Nullable<int> PrevilegeLevel { get; set; }
    
        public virtual Action Action { get; set; }
        public virtual Controller Controller { get; set; }
        public virtual Role Role { get; set; }
    }
}
