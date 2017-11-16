# 请假系统Web端部署说明
## 部署前注意修改web.config
> * connectionStrings -> imaw_qingjiaEntities   
>修改服务器名称
> * appSettings -> FreshmanYear   
> 值为需要早晚自习请假年级（大一年级） 例：2017
> * appSettings -> ShortMessageService  
> 是否开启短信通知服务？ -1代表发发送测试人员  1代表 开启服务 其他 代表关闭服务
> * appSettings -> Tel  
> 值为测试人员手机号码  