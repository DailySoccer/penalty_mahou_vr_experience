import android.app.Activity;
 
import com.ad4screen.sdk.A4S;
import com.facebook.unity.FBUnityPlayerActivity;

public class MyA4SActivity extends FBUnityPlayerActivity {
	private A4S mA4S;
 
	@Override
	protected void onCreate(Bundle savedInstanceState) {
		super.onCreate(savedInstanceState);
		//mA4S = getA4S();
	}
 
	@Override
	protected void onResume() {
		super.onResume();
		//getA4S().startActivity(this);
	}
	@Override
	protected void onNewIntent(Intent newIntent) {
		this.setIntent(newIntent);
	}
 
	@Override
	protected void onPause() {
		super.onPause();
		//getA4S().stopActivity(this);
	}
 
	public A4S getA4S() {
                mA4S = A4S.get(this);
		return mA4S;
	}
 
}