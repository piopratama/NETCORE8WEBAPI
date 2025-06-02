import React, { useEffect, useState } from 'react';
import './App.css';
import * as msal from '@azure/msal-browser';

const msalInstance = new msal.PublicClientApplication({
	auth: {
		clientId: '77052093-9bed-465f-a699-7f4abb3229df',
		authority: 'https://login.microsoftonline.com/3c65f370-dc3a-4cf7-ac9e-af7e53636aea',
		redirectUri: 'http://localhost:5173',
	},
	cache: {
		cacheLocation: 'sessionStorage',
		storeAuthStateInCookie: false,
	},
});

function App(): JSX.Element {
	const [accessToken, setAccessToken] = useState<string | null>(null);
	const [isInitialized, setIsInitialized] = useState(false);
	const [apiResult, setApiResult] = useState<string | null>(null);

	useEffect(() => {
		msalInstance.initialize()
			.then(() => {
				console.log("✅ MSAL initialized");
				setIsInitialized(true);
			})
			.catch(err => console.error("MSAL init error:", err));
	}, []);

	const handleLogin = async () => {
		try {
			if (!isInitialized) return;

			const loginResponse = await msalInstance.loginPopup({
				scopes: ['api://3d6f02fd-479d-4971-a4fa-d7b4e6871678/access_as_user'],
			});

			setAccessToken(loginResponse.accessToken);
			console.log("✅ Access Token:", loginResponse.accessToken);
		} catch (err) {
			console.error("❌ Login error:", err);
		}
	};

	const handleLogout = async () => {
		try {
			const currentAccounts = msalInstance.getAllAccounts();
			if (currentAccounts.length > 0) {
				await msalInstance.logoutPopup({
					account: currentAccounts[0],
				});
				setAccessToken(null);
				setApiResult(null);
			}
		} catch (err) {
			console.error("❌ Logout error:", err);
		}
	};

	const testWithToken = async () => {
		try {
			const res = await fetch('https://localhost:7241/secure-data', {
				headers: {
					Authorization: `Bearer ${accessToken}`,
				},
			});

			const data = await res.json();
			setApiResult(`✅ Status: ${res.status}\n${JSON.stringify(data, null, 2)}`);
		} catch (err) {
			setApiResult(`❌ Error: ${err}`);
		}
	};

	const testWithoutToken = async () => {
		try {
			const res = await fetch('https://localhost:7241/secure-data');
			const text = await res.text();
			setApiResult(`⛔ Status: ${res.status}\n${text}`);
		} catch (err) {
			setApiResult(`❌ Error: ${err}`);
		}
	};

	return (
		<div className="App">
			<h1>🔐 Microsoft Login Demo</h1>

			{!accessToken ? (
				<button onClick={handleLogin} disabled={!isInitialized}>
					Login with Microsoft
				</button>
			) : (
				<button onClick={handleLogout}>Logout</button>
			)}

			{accessToken && (
				<div style={{ marginTop: '20px' }}>
					<p>✅ You are logged in!</p>
					<code style={{ wordWrap: 'break-word', whiteSpace: 'pre-wrap' }}>
						{accessToken}
					</code>
				</div>
			)}

			<div style={{ marginTop: '40px' }}>
				<h3>🔎 Coba akses endpoint `/secure-data`:</h3>
				<button onClick={testWithoutToken}>Tanpa Token</button>{' '}
				<button onClick={testWithToken} disabled={!accessToken}>Dengan Token</button>

				{apiResult && (
					<pre style={{ textAlign: 'left', marginTop: '20px', background: '#f4f4f4', padding: '1rem' }}>
						{apiResult}
					</pre>
				)}
			</div>
		</div>
	);
}

export default App;
