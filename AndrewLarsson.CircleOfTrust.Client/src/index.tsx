import { StrictMode } from 'react'
import { createRoot } from 'react-dom/client'
import { AuthProvider } from './auth/AuthContext.tsx'
import { BrowserRouter as Router } from "react-router-dom";
import App from './App.tsx'
import './index.css'

createRoot(document.getElementById('root')!).render(
	<StrictMode>
		<AuthProvider>
			<Router>
				<App />
			</Router>
		</AuthProvider>
	</StrictMode>
);
