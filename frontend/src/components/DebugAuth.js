import { useAuth } from '@/context/AuthContext'
import { useUserData } from '@/hooks/useUserData'
import { Box, Typography, Paper } from '@mui/material'

const DebugAuth = () => {
	const { user: authUser, token: authToken, isAuthenticated } = useAuth()
	const { user: hookUser } = useUserData()

	return (
		<Paper sx={{ p: 2, mb: 2, bgcolor: 'yellow.100' }}>
			<Typography variant="h6" gutterBottom>
				Debug Info
			</Typography>
			<Box sx={{ display: 'flex', gap: 4 }}>
				<Box>
					<Typography variant="subtitle2">AuthContext:</Typography>
					<Typography variant="body2">isAuthenticated: {String(isAuthenticated)}</Typography>
					<Typography variant="body2">user: {JSON.stringify(authUser, null, 2)}</Typography>
					<Typography variant="body2">token: {authToken ? authToken.substring(0, 20) + '...' : 'null'}</Typography>
				</Box>
				<Box>
					<Typography variant="subtitle2">useUserData Hook:</Typography>
					<Typography variant="body2">user: {JSON.stringify(hookUser, null, 2)}</Typography>
				</Box>
			</Box>
		</Paper>
	)
}

export default DebugAuth
