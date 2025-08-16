import { useState } from 'react'
import { useRouter } from 'next/router'
import { useAuth } from '@/context/AuthContext'
import {
	Container,
	Box,
	Typography,
	TextField,
	Button,
	Paper,
	Alert,
	CircularProgress,
} from '@mui/material'

const Login = ()  => {

	const [username, setUsername] = useState('john')
	const [password, setPassword] = useState('password')
	const [submitting, setSubmitting] = useState(false)
	const [errorMsg, setErrorMsg] = useState('')
	const { login } = useAuth()
	const router = useRouter()

	const handleSubmit = async (e) => {
		
		e.preventDefault()
		setErrorMsg('')
		setSubmitting(true)
		try {
	
			await login(username, password)
			router.push('/dashboard')
		} catch (error) {
			const status = error?.response?.status
			if (status === 401) {
				setErrorMsg('Credenciales inválidas')
			} else if (status === 500) {
				setErrorMsg('Error del servidor (500)')
			} else {
				setErrorMsg('No se pudo iniciar sesión')
			}
		} finally {
			setSubmitting(false)
		}
	}

	return (
		<Container
			component="main"
			maxWidth="xs"
			sx={{
				display: 'flex',
				flexDirection: 'column',
				justifyContent: 'center',
				alignItems: 'center',
				minHeight: '100vh',
			}}
		>
			<Paper
				elevation={3}
				sx={{
					padding: 4,
					display: 'flex',
					flexDirection: 'column',
					alignItems: 'center',
					width: '100%',
				}}
			>
				<Typography component="h1" variant="h5">
					Login
				</Typography>
				{errorMsg && (
					<Alert severity="error" sx={{ width: '100%', mt: 2 }}>
						{errorMsg}
					</Alert>
				)}
				<Box
					component="form"
					onSubmit={handleSubmit}
					noValidate
					sx={{ mt: 1, width: '100%' }}
				>
					<TextField
						margin="normal"
						required
						fullWidth
						id="username"
						label="Username"
						name="username"
						autoComplete="username"
						autoFocus
						value={username}
						onChange={(e) => setUsername(e.target.value)}
					
					/>
					<TextField
						margin="normal"
						required
						fullWidth
						name="password"
						label="Password"
						type="password"
						id="password"
						autoComplete="current-password"
						value={password}
						onChange={(e) => setPassword(e.target.value)}
					
					/>
					<Button
						type="submit"
						fullWidth
						variant="contained"
						sx={{ mt: 3, mb: 2 }}
						disabled={submitting}
					>
						{submitting ? <CircularProgress size={22} /> : 'Sign In'}
					</Button>
				</Box>
			</Paper>
		</Container>
	)
}


export default Login