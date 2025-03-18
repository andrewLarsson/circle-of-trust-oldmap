import { createContext, useState, ReactNode } from "react";

export type AuthContextType = {
	isAuthenticated: boolean;
	authenticationToken: string | null;
	setAuthenticationToken: (token: string) => void;
};

export const AuthContext = createContext<AuthContextType | undefined>(undefined);

export const AuthProvider = ({ children }: { children: ReactNode }) => {
	const [authenticationToken, setAuthenticationToken] = useState<string | null>(null);
	const isAuthenticated = authenticationToken !== null;
	return (
		<AuthContext.Provider value={{ authenticationToken, setAuthenticationToken, isAuthenticated }}>
			{children}
		</AuthContext.Provider>
	);
};
