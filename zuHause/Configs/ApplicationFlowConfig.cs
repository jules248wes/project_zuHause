﻿namespace zuHause.Configs
{
    public class ApplicationFlowConfig
    {
        public static readonly Dictionary<string, List<string>> ApplicationStepsMap = new()
        {
            ["HOUSE_VIEWING"] = new() // 看房
            {
                "APPLIED",       // 已申請
                "PENDING",       // 待審核
                "APPROVED",      // 已同意申請
                "VIEWING_SCHEDULED",       // 看房日
                "VIEWING_COMPLETED"         // 看房已完成
            },
            ["RENTAL"] = new() // 租賃
            {

                "APPLIED", // 已申請
                "PENDING", // 待審核
                "WAITING_CONTRACT", // 租約編輯中
                "SIGNING", // 待租客簽署
                "WAIT_TENANT_AGREE", // 待租客同意
                "WAIT_LANDLORD_AGREE", // 待房東同意
                "CONTRACTED", // 租約已成立
            },
            ["REJECTED_FLOW"] = new() // 看房/租賃被拒絕
            {
                "APPLIED",
                "PENDING",
                "REJECTED"
            }
        };
    }
}
