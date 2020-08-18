-- automatically generated by the FlatBuffers compiler, do not modify

-- namespace: fb

local flatbuffers = require('flatbuffers')

local C_SkillDataTR = {} -- the module
local C_SkillDataTR_mt = {} -- the class metatable

function C_SkillDataTR.New()
    local o = {}
    setmetatable(o, {__index = C_SkillDataTR_mt})
    return o
end
function C_SkillDataTR.GetRootAsC_SkillDataTR(buf, offset)
    local n = flatbuffers.N.UOffsetT:Unpack(buf, offset)
    local o = C_SkillDataTR.New()
    o:Init(buf, n + offset)
    return o
end
function C_SkillDataTR_mt:Init(buf, pos)
    self.view = flatbuffers.view.New(buf, pos)
end
function C_SkillDataTR_mt:_id()
    local o = self.view:Offset(4)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR_mt:Name()
    local o = self.view:Offset(6)
    if o ~= 0 then
        return self.view:String(o + self.view.pos)
    end
end
function C_SkillDataTR_mt:Description()
    local o = self.view:Offset(8)
    if o ~= 0 then
        return self.view:String(o + self.view.pos)
    end
end
function C_SkillDataTR_mt:CoolTime()
    local o = self.view:Offset(10)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR_mt:CostSP()
    local o = self.view:Offset(12)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR_mt:AttackDistance()
    local o = self.view:Offset(14)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Float32, o + self.view.pos)
    end
    return 0.0
end
function C_SkillDataTR_mt:AttackAngle()
    local o = self.view:Offset(16)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Float32, o + self.view.pos)
    end
    return 0.0
end
function C_SkillDataTR_mt:AttackTargetTags(j)
    local o = self.view:Offset(18)
    if o ~= 0 then
        local a = self.view:Vector(o)
        return self.view:String(a + ((j-1) * 4))
    end
    return ''
end
function C_SkillDataTR_mt:AttackTargetTagsLength()
    local o = self.view:Offset(18)
    if o ~= 0 then
        return self.view:VectorLen(o)
    end
    return 0
end
function C_SkillDataTR_mt:ImpactType(j)
    local o = self.view:Offset(20)
    if o ~= 0 then
        local a = self.view:Vector(o)
        return self.view:String(a + ((j-1) * 4))
    end
    return ''
end
function C_SkillDataTR_mt:ImpactTypeLength()
    local o = self.view:Offset(20)
    if o ~= 0 then
        return self.view:VectorLen(o)
    end
    return 0
end
function C_SkillDataTR_mt:NextBattlerId()
    local o = self.view:Offset(22)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR_mt:AtkRatio()
    local o = self.view:Offset(24)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Float32, o + self.view.pos)
    end
    return 0.0
end
function C_SkillDataTR_mt:DurationTime()
    local o = self.view:Offset(26)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Float32, o + self.view.pos)
    end
    return 0.0
end
function C_SkillDataTR_mt:AtkInterval()
    local o = self.view:Offset(28)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Float32, o + self.view.pos)
    end
    return 0.0
end
function C_SkillDataTR_mt:SkillPrefab()
    local o = self.view:Offset(30)
    if o ~= 0 then
        return self.view:String(o + self.view.pos)
    end
end
function C_SkillDataTR_mt:AnimationName()
    local o = self.view:Offset(32)
    if o ~= 0 then
        return self.view:String(o + self.view.pos)
    end
end
function C_SkillDataTR_mt:HitFxPrefab()
    local o = self.view:Offset(34)
    if o ~= 0 then
        return self.view:String(o + self.view.pos)
    end
end
function C_SkillDataTR_mt:Level()
    local o = self.view:Offset(36)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR_mt:AttackType()
    local o = self.view:Offset(38)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR_mt:SelectorType()
    local o = self.view:Offset(40)
    if o ~= 0 then
        return self.view:Get(flatbuffers.N.Int32, o + self.view.pos)
    end
    return 0
end
function C_SkillDataTR.Start(builder) builder:StartObject(19) end
function C_SkillDataTR.Add_id(builder, Id) builder:PrependInt32Slot(0, Id, 0) end
function C_SkillDataTR.AddName(builder, Name) builder:PrependUOffsetTRelativeSlot(1, Name, 0) end
function C_SkillDataTR.AddDescription(builder, Description) builder:PrependUOffsetTRelativeSlot(2, Description, 0) end
function C_SkillDataTR.AddCoolTime(builder, CoolTime) builder:PrependInt32Slot(3, CoolTime, 0) end
function C_SkillDataTR.AddCostSP(builder, CostSP) builder:PrependInt32Slot(4, CostSP, 0) end
function C_SkillDataTR.AddAttackDistance(builder, AttackDistance) builder:PrependFloat32Slot(5, AttackDistance, 0.0) end
function C_SkillDataTR.AddAttackAngle(builder, AttackAngle) builder:PrependFloat32Slot(6, AttackAngle, 0.0) end
function C_SkillDataTR.AddAttackTargetTags(builder, AttackTargetTags) builder:PrependUOffsetTRelativeSlot(7, AttackTargetTags, 0) end
function C_SkillDataTR.StartAttackTargetTagsVector(builder, numElems) return builder:StartVector(4, numElems, 4) end
function C_SkillDataTR.AddImpactType(builder, ImpactType) builder:PrependUOffsetTRelativeSlot(8, ImpactType, 0) end
function C_SkillDataTR.StartImpactTypeVector(builder, numElems) return builder:StartVector(4, numElems, 4) end
function C_SkillDataTR.AddNextBattlerId(builder, NextBattlerId) builder:PrependInt32Slot(9, NextBattlerId, 0) end
function C_SkillDataTR.AddAtkRatio(builder, AtkRatio) builder:PrependFloat32Slot(10, AtkRatio, 0.0) end
function C_SkillDataTR.AddDurationTime(builder, DurationTime) builder:PrependFloat32Slot(11, DurationTime, 0.0) end
function C_SkillDataTR.AddAtkInterval(builder, AtkInterval) builder:PrependFloat32Slot(12, AtkInterval, 0.0) end
function C_SkillDataTR.AddSkillPrefab(builder, SkillPrefab) builder:PrependUOffsetTRelativeSlot(13, SkillPrefab, 0) end
function C_SkillDataTR.AddAnimationName(builder, AnimationName) builder:PrependUOffsetTRelativeSlot(14, AnimationName, 0) end
function C_SkillDataTR.AddHitFxPrefab(builder, HitFxPrefab) builder:PrependUOffsetTRelativeSlot(15, HitFxPrefab, 0) end
function C_SkillDataTR.AddLevel(builder, Level) builder:PrependInt32Slot(16, Level, 0) end
function C_SkillDataTR.AddAttackType(builder, AttackType) builder:PrependInt32Slot(17, AttackType, 0) end
function C_SkillDataTR.AddSelectorType(builder, SelectorType) builder:PrependInt32Slot(18, SelectorType, 0) end
function C_SkillDataTR.End(builder) return builder:EndObject() end

return C_SkillDataTR -- return the module