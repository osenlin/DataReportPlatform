namespace net91com.Reports.Services.ServicesBaseEntity
{
    public enum RightEnum
    {
        /// <summary>
        ///     不检查
        /// </summary>
        NoCheck = 0,

        /// <summary>
        ///     只用检查地址
        /// </summary>
        OnlyUrlRight = 1,

        /// <summary>
        ///     地址和产品都检查
        /// </summary>
        UrlAndSoftRight = 2,

        /// <summary>
        ///     地址和项目来源都检查
        /// </summary>
        UrlAndProjectSourceRight = 3,

        /// <summary>
        ///     自定义权限验证
        /// </summary>
        DefinedByYourself = 4,

        DefinedByYourselfAndUrlAndSoftRight = 5
    }
}