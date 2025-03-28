import { useState, useEffect } from "react";
import { useNavigate, useLocation } from "react-router-dom";
import { useAuth } from './auth/useAuth';
import "./Login.css";

const Login = (): JSX.Element => {
	const { setAuthenticationToken, isAuthenticated } = useAuth();
	const [googleLoaded, setGoogleLoaded] = useState(false);
	const navigate = useNavigate();
	const from = (useLocation().state as { from?: string })?.from || "/";

	useEffect(() => {
		if (isAuthenticated) {
			navigate(from, {
				replace: true
			});
		}
	}, [isAuthenticated, navigate, from]);

	useEffect(() => {
		const script = document.createElement("script");
		script.src = "https://accounts.google.com/gsi/client";
		script.async = true;
		script.defer = true;
		script.onload = () => setGoogleLoaded(true);
		document.body.appendChild(script);
		return () => {
			document.body.removeChild(script);
		};
	}, []);

	useEffect(() => {
		if (!googleLoaded || !window.google?.accounts) return;
		window.google.accounts.id.initialize({
			client_id: "68033942219-fshhuhthacrvi256dmca0ok4relukd76.apps.googleusercontent.com",
			callback: (response: google.accounts.id.CredentialResponse) => {
				setAuthenticationToken(response.credential);
			}
		});
		const buttonContainer = document.getElementById("google-signin-btn");
		window.google.accounts.id.renderButton(
			buttonContainer as HTMLElement,
			{ theme: "outline", size: "large", type: "standard" }
		);
	}, [googleLoaded, setAuthenticationToken, navigate, from]);

	if (isAuthenticated) return <></>;
	return (
		<div className="container">
			<h2 className="title">Login</h2>
			<div id="google-signin-btn"></div>
		</div>
	);
};

export default Login;
