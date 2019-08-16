# Self Healing Wcf Client

Most of this code is old. Basically, this wraps a proxy around the WCF client, which, at least at the time, had some pretty bad exception handling situations. If a WCF channel faulted, it'd be stuck FOREVER, and there'd be code strewn about the code base that doesn't do anything except try again, or reconnect WCF, or whatever. In the code, I didn't care. I just wanted it to work. So I made it work. The result is what you see here. 
