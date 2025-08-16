import { useEffect, useState } from 'react'
import { useRouter } from 'next/router'
import WithAuth from '@/hoc/withAuth'
import { useAuth } from '@/context/AuthContext'
import { useNavigation } from '@/hooks/useNavigation'
import { usePropertyTrace } from '@/hooks/usePropertyTrace'
import axios from 'axios'
import {
	Container,
	Typography,
	Box,
	Grid,
	Card,
	CardContent,
	Paper,
	Button,
	CardMedia,
	ImageList,
	ImageListItem,
	Dialog,
	DialogTitle,
	DialogContent,
	DialogActions,
	IconButton,
	Chip,
	Divider,
	CircularProgress,
	Alert
} from '@mui/material'
import {
	ArrowBack as ArrowBackIcon,
	Close as CloseIcon,
	ShoppingCart as ShoppingCartIcon,
	Home as HomeIcon,
	LocationOn as LocationOnIcon,
	CalendarToday as CalendarTodayIcon,
	AttachMoney as AttachMoneyIcon
} from '@mui/icons-material'

const PropertyDetailsPage = () => {
	const { selectedProperty, setSelectedProperty, user } = useAuth()
	const { handleGoBack, handleViewDashboard } = useNavigation()
	const { createPurchaseTrace, getAllPropertyTraces, loading: traceLoading } = usePropertyTrace()
	const router = useRouter()
	const [selectedImage, setSelectedImage] = useState(null)
	const [openImageDialog, setOpenImageDialog] = useState(false)
	const [purchaseDialogOpen, setPurchaseDialogOpen] = useState(false)
	const [isProcessingPurchase, setIsProcessingPurchase] = useState(false)
	const [purchaseError, setPurchaseError] = useState(null)
	const [purchaseSuccess, setPurchaseSuccess] = useState(false)
	const [allPropertyTraces, setAllPropertyTraces] = useState([])

	// Redirigir si no hay propiedad seleccionada
	useEffect(() => {
		if (!selectedProperty) {
			router.push('/dashboard')
		}
	}, [selectedProperty, router])

	if (!selectedProperty) {
		return (
			<Box sx={{ 
				minHeight: '100vh', 
				display: 'flex', 
				alignItems: 'center', 
				justifyContent: 'center' 
			}}>
				<Typography>Cargando...</Typography>
			</Box>
		)
	}

	const handleImageClick = (image) => {
		setSelectedImage(image)
		setOpenImageDialog(true)
	}

	const handleCloseImageDialog = () => {
		setOpenImageDialog(false)
		setSelectedImage(null)
	}

	const handlePurchaseProperty = () => {
		setPurchaseDialogOpen(true)
	}

	const handleConfirmPurchase = async () => {
		if (!selectedProperty) return

		if (!user || !user.id) {
			setPurchaseError('Error: Usuario no válido para realizar la compra.')
			return
		}

		setIsProcessingPurchase(true)
		setPurchaseError(null)
		setPurchaseSuccess(false)

		try {
			console.log('Attempting to update property - user.id:', user.id, 'type:', typeof user.id, 'property.id:', selectedProperty.id)
			
			// Realizar todas las operaciones: actualizar forSale, ownerId y crear PropertyTrace
			await Promise.all([
				// 1. Actualizar forSale a false
				axios.patch(`/api/properties/update-for-sale?propertyId=${selectedProperty.id}`, {
					forSale: false
				}),
				// 2. Actualizar ownerId al usuario actual
				axios.patch(`/api/properties/update-owner?propertyId=${selectedProperty.id}`, {
					ownerId: user.id
				}),
				// 3. Crear el registro de compra en PropertyTrace
				createPurchaseTrace(selectedProperty, user.name)
			])

			// Actualizar la propiedad en el contexto local
			const updatedProperty = {
				...selectedProperty,
				forSale: false,
				ownerId: user.id,
				ownerName: user.name
			}
			setSelectedProperty(updatedProperty)

			// Obtener todos los PropertyTraces después de la compra exitosa
			try {
				const traces = await getAllPropertyTraces()
				setAllPropertyTraces(traces)
				console.log('PropertyTraces retrieved after purchase:', traces)
			} catch (traceError) {
				console.error('Error retrieving PropertyTraces:', traceError)
			}

			// Mostrar mensaje de éxito
			setPurchaseSuccess(true)

			// Cerrar el diálogo y regresar al dashboard después de un breve delay
			setTimeout(() => {
				setPurchaseDialogOpen(false)
				setPurchaseSuccess(false)
				// Regresar al dashboard después de la compra exitosa
				handleViewDashboard()
			}, 2000)

		} catch (error) {
			console.error('Error al procesar la compra:', error)
			
			// Manejar diferentes tipos de errores
			let errorMessage = 'Error al procesar la compra. Inténtalo de nuevo.'
			
			if (error.response?.status === 404) {
				errorMessage = 'La propiedad no fue encontrada.'
			} else if (error.response?.status === 401) {
				errorMessage = 'No tienes autorización para realizar esta compra.'
			} else if (error.response?.data?.message) {
				errorMessage = error.response.data.message
			}
			
			setPurchaseError(errorMessage)
		} finally {
			setIsProcessingPurchase(false)
		}
	}

	const handleClosePurchaseDialog = () => {
		if (!isProcessingPurchase) {
			setPurchaseDialogOpen(false)
			setPurchaseError(null)
			setPurchaseSuccess(false)
		}
	}

	return (
		<Box sx={{ minHeight: '100vh', bgcolor: 'grey.50' }}>
			{/* Header */}
			<Paper elevation={1} sx={{ p: 2, mb: 3 }}>
				<Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
					<Box sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
						<IconButton onClick={handleGoBack} size="large">
							<ArrowBackIcon />
						</IconButton>
						<Typography variant="h5">
							Detalles de la Propiedad
						</Typography>
					</Box>
					<Button
						variant="outlined"
						onClick={handleViewDashboard}
						startIcon={<HomeIcon />}
					>
						Volver al Dashboard
					</Button>
				</Box>
			</Paper>

			<Container maxWidth="xl">
				<Grid container spacing={4}>
					{/* Información Principal */}
					<Grid item xs={12} md={6}>
						<Paper elevation={2} sx={{ p: 3, mb: 3 }}>
							<Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
								<HomeIcon color="primary" />
								<Typography variant="h4" component="h1">
									{selectedProperty.name}
								</Typography>
							</Box>
							
							<Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
								<LocationOnIcon color="secondary" />
								<Typography variant="h6" color="text.secondary">
									{selectedProperty.address}
								</Typography>
							</Box>

							<Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
								<AttachMoneyIcon color="primary" />
								<Typography variant="h3" color="primary" fontWeight="bold">
									${selectedProperty.price.toLocaleString()}
								</Typography>
							</Box>

							<Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 3 }}>
								<CalendarTodayIcon color="action" />
								<Typography variant="body1">
									Año de construcción: {selectedProperty.year}
								</Typography>
							</Box>

							<Box sx={{ mb: 3 }}>
								<Typography variant="body1" color="text.secondary">
									<strong>Propietario:</strong> {
										selectedProperty.ownerId === user?.id 
											? `${user.name} (Tú)` 
											: selectedProperty.ownerName || 'Sin propietario'
									}
								</Typography>
							</Box>

							<Box sx={{ mb: 3 }}>
								{selectedProperty.forSale ? (
									<Chip 
										label="¡Disponible para comprar!" 
										color="success" 
										size="large"
										sx={{ fontSize: '1rem', py: 2 }}
									/>
								) : (
									<Chip 
										label="No disponible para compra" 
										color="default" 
										size="large"
									/>
								)}
							</Box>

							<Divider sx={{ my: 3 }} />

							{/* Botón de Compra */}
							{selectedProperty.forSale && (
								<Button
									variant="contained"
									size="large"
									fullWidth
									startIcon={<ShoppingCartIcon />}
									onClick={handlePurchaseProperty}
									sx={{ 
										py: 2, 
										fontSize: '1.1rem',
										background: 'linear-gradient(45deg, #4CAF50 30%, #81C784 90%)',
										'&:hover': {
											background: 'linear-gradient(45deg, #45a049 30%, #66BB6A 90%)',
										}
									}}
								>
									Adquirir esta Propiedad
								</Button>
							)}
						</Paper>
					</Grid>

					{/* Galería de Imágenes */}
					<Grid item xs={12} md={6}>
						<Paper elevation={2} sx={{ p: 3 }}>
							<Typography variant="h5" gutterBottom>
								Galería de Imágenes ({selectedProperty.images?.length || 0})
							</Typography>
							
							{selectedProperty.images && selectedProperty.images.length > 0 ? (
								<ImageList 
									variant="masonry" 
									cols={2} 
									gap={8}
									sx={{ maxHeight: 600, overflow: 'auto' }}
								>
									{selectedProperty.images.map((image, index) => (
										<ImageListItem key={image.id || index}>
											<Card 
												elevation={1} 
												sx={{ 
													cursor: 'pointer',
													transition: 'transform 0.2s',
													'&:hover': {
														transform: 'scale(1.02)'
													}
												}}
												onClick={() => handleImageClick(image)}
											>
												<CardMedia
													component="img"
													image={image.fileUrl || image.file}
													alt={`Imagen ${index + 1} de ${selectedProperty.name}`}
													sx={{ 
														height: 200,
														objectFit: 'cover'
													}}
												/>
											</Card>
										</ImageListItem>
									))}
								</ImageList>
							) : (
								<Box sx={{ 
									textAlign: 'center', 
									py: 8,
									bgcolor: 'grey.100',
									borderRadius: 2
								}}>
									<Typography variant="h6" color="text.secondary">
										No hay imágenes disponibles para esta propiedad
									</Typography>
								</Box>
							)}
						</Paper>
					</Grid>
				</Grid>
			</Container>

			{/* Dialog para ver imagen en grande */}
			<Dialog
				open={openImageDialog}
				onClose={handleCloseImageDialog}
				maxWidth="lg"
				fullWidth
			>
				<DialogTitle sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
					<Typography variant="h6">
						{selectedProperty.name}
					</Typography>
					<IconButton onClick={handleCloseImageDialog}>
						<CloseIcon />
					</IconButton>
				</DialogTitle>
				<DialogContent>
					{selectedImage && (
						<Box sx={{ textAlign: 'center' }}>
							<img
								src={selectedImage.fileUrl || selectedImage.file}
								alt="Imagen ampliada"
								style={{
									maxWidth: '100%',
									maxHeight: '70vh',
									objectFit: 'contain'
								}}
							/>
						</Box>
					)}
				</DialogContent>
			</Dialog>

			{/* Dialog de confirmación de compra */}
			<Dialog
				open={purchaseDialogOpen}
				onClose={handleClosePurchaseDialog}
				maxWidth="sm"
				fullWidth
			>
				<DialogTitle>
					<Typography variant="h5">
						{purchaseSuccess ? '¡Compra Exitosa!' : 'Confirmar Compra'}
					</Typography>
				</DialogTitle>
				<DialogContent>
					{purchaseSuccess ? (
						<Box sx={{ textAlign: 'center', py: 2 }}>
							<Alert severity="success" sx={{ mb: 2 }}>
								¡Felicitaciones! Has adquirido la propiedad exitosamente.
							</Alert>
							<Typography variant="body1" gutterBottom>
								Ahora eres el propietario de esta propiedad.
							</Typography>
							<Typography variant="body2" color="text.secondary">
								La propiedad ya no está disponible para otros compradores.
							</Typography>
							
							{allPropertyTraces.length > 0 && (
								<Box sx={{ mt: 2, p: 2, bgcolor: 'background.default', borderRadius: 1 }}>
									<Typography variant="h6" sx={{ mb: 1 }}>
										PropertyTraces en el Sistema:
									</Typography>
									<Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
										Total de compras registradas: {allPropertyTraces.length}
									</Typography>
									{allPropertyTraces.slice(0, 3).map((trace, index) => (
										<Typography key={trace.id} variant="body2" sx={{ mb: 0.5 }}>
											• {trace.name} - ${trace.value.toLocaleString()} ({new Date(trace.dateSale).toLocaleDateString()})
										</Typography>
									))}
									{allPropertyTraces.length > 3 && (
										<Typography variant="body2" color="text.secondary">
											... y {allPropertyTraces.length - 3} más
										</Typography>
									)}
								</Box>
							)}
							
							<Typography variant="body2" color="primary" sx={{ mt: 1, fontStyle: 'italic' }}>
								Regresando al dashboard...
							</Typography>
						</Box>
					) : (
						<>
							{purchaseError && (
								<Alert severity="error" sx={{ mb: 2 }}>
									{purchaseError}
								</Alert>
							)}
							<Typography variant="body1" gutterBottom>
								¿Estás seguro de que deseas adquirir esta propiedad?
							</Typography>
							<Box sx={{ mt: 2, p: 2, bgcolor: 'grey.100', borderRadius: 1 }}>
								<Typography variant="h6">{selectedProperty.name}</Typography>
								<Typography variant="body2" color="text.secondary">
									{selectedProperty.address}
								</Typography>
								<Typography variant="h5" color="primary" sx={{ mt: 1 }}>
									${selectedProperty.price.toLocaleString()}
								</Typography>
							</Box>
						</>
					)}
				</DialogContent>
				{!purchaseSuccess && (
					<DialogActions sx={{ p: 3 }}>
						<Button 
							onClick={handleClosePurchaseDialog} 
							size="large"
							disabled={isProcessingPurchase}
						>
							Cancelar
						</Button>
						<Button 
							onClick={handleConfirmPurchase} 
							variant="contained" 
							size="large"
							disabled={isProcessingPurchase}
							startIcon={isProcessingPurchase ? <CircularProgress size={20} /> : <ShoppingCartIcon />}
						>
							{isProcessingPurchase ? 'Procesando...' : 'Confirmar Compra'}
						</Button>
					</DialogActions>
				)}
			</Dialog>
		</Box>
	)
}

export default WithAuth(PropertyDetailsPage)
