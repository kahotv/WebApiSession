# WebApi Session

## Session示例

​	先来看看三个接口的例子

| 接口       | 需要登陆 | 需要检查api权限 | Session标记      |
| ---------- | -------- | --------------- | ---------------- |
| user/login |          |                 |                  |
| user/my    | √        |                 | [Session(false)] |
| user/my2   | √        | √               | [Session(true)]  |

​	`user/login`任何人都能调用

```c#
[HttpPost]
[Route("login")]
public JObject Login([FromBody] JObject param)
{
	//...(略)
}
```



​	`user/my`需要登陆后才能调用。

```c#
[HttpPost, Session(false)]
[Route("my")]
public JObject My([FromBody] JObject param)
{
	JObject jobj = new JObject();

	//从缓存得到user数据并返回
	JObject jdata = new JObject();
	jdata["token"] = Token;
	jdata["id"] = this.UserData.UserInfo.UserID;
	jdata["name"] = this.UserData.UserInfo.UserName;
	jdata["truename"] = this.UserData.UserInfo.TrueName;

	jobj["errcode"] = 0;
	jobj["data"] = jdata;

	return jobj;
}

```

​	`user/my2`需要登陆，并且分配了权限才能调用。

```c#
[HttpPost, Session(true)]
[Route("my2")]
public JObject My2([FromBody] JObject param)
{
	return My(param);
}
```



​	本文核心功能是实现`user/my`接口那样可以从`this.UserData.UserInfo`拿到登录者的数据。

## Session实现

​	这里给出每个类的作用，具体代码见git

| 类名              | 作用                                                         |
| ----------------- | ------------------------------------------------------------ |
| SessionAttribute  | 从Header里得到Token，再拿Token从SessionManager里拿到已登陆的User |
| SessionController | 拓展ApiController类，储存额外数据（比如UserSession）         |
| SessionManager    | 管理在线用户                                                 |
| UserEntity        | 储存用户信息                                                 |
| UserSession       | 储存用户登陆信息和UserEntity                                 |

## 使用Session

### 服务端

​	业务Controller继承`SessionController`，然后配合`Session`标记

```c#
[RoutePrefix("user")]
public class UserController : SessionController
{
	[HttpPost]
	[Route("login")]
	public JObject Login([FromBody] JObject param)
	{
		JObject jobj = new JObject();
        //...略
        SessionManager.Instance().AddSession(user, CallIP, out string token);
        jobj["token"] = token;
        return jobj;
	}
	
	[HttpPost, Session(false)]
	[Route("logout")]
	public JObject Logout([FromBody] JObject param)
	{
		JObject jobj = new JObject();

		SessionManager.Instance().RemoveSession(Token);
		jobj["errcode"] = 0;

		return jobj;
	}
	//...略
}
```

### 客户端

​	登陆成功后，会得到token。

```json
#/user/login
#返回
{
    //...略
	"token":"C9FAE624-6D0E-4301-B179-74BCE0040636"
}
```

​	访问其他接口时，Header里带上token。

```json
#/user/my
#请求
##Header
Content-Type: application/json
X-Token: C9FAE624-6D0E-4301-B179-74BCE0040636
#X-Token在SessionAttribute类里使用
```
