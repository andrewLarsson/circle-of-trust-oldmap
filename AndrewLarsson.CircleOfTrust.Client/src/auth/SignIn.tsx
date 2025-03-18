import { useState, useEffect } from "react";
import { useAuth } from './useAuth';

export const SignIn = (): JSX.Element => {
    const { setAuthenticationToken, isAuthenticated } = useAuth();
    const [googleLoaded, setGoogleLoaded] = useState(false);

    useEffect(() => {
        const script = document.createElement("script");
        script.src = "https://accounts.google.com/gsi/client";
        script.async = true;
        script.defer = true;
        script.onload = () => setGoogleLoaded(true);
        document.body.appendChild(script);

        // Cleanup function to remove the script when component unmounts
        return () => {
            document.body.removeChild(script);
        };
    }, []);

    useEffect(() => {
        if (googleLoaded && window.google?.accounts) {
            window.google.accounts.id.initialize({
                client_id: "68033942219-fshhuhthacrvi256dmca0ok4relukd76.apps.googleusercontent.com",
                callback: (response: google.accounts.id.CredentialResponse) => setAuthenticationToken(response.credential),
            });
            const buttonContainer = document.getElementById("google-signin-btn");
            if (buttonContainer) {
                window.google.accounts.id.renderButton(
                    buttonContainer as HTMLElement,
                    { theme: "outline", size: "large", type: "standard" }
                );
            }
        }
    }, [googleLoaded, setAuthenticationToken]);

    if (isAuthenticated) return <></>;
    return (
        <div className="border p-4 rounded-lg">
            <h2 className="font-semibold">Sign In</h2>
            <div id="google-signin-btn"></div>
        </div>
    );
};
